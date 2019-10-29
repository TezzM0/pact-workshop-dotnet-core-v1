param(
    $buildMetadataFile = "package.json",

    $workingDir = (Get-Location),

    $pactsDir = (Join-Path $workingDir "pacts"),

    $pactBroker = "https://test_als_terry.pact.dius.com.au",

    $templateUrl = "$pactBroker/pacts/provider/:provider/consumer/:consumer/version/:version",

    $tagTemplateUrl = "$pactBroker/pacticipants/:consumer/versions/:version/tags/:tag"
)

$ErrorActionPreference = "Stop"

trap
{
    # this script is called via -File in the powershell params which will not always return a valid exit code on exception
    # catch any exception and force the exit code.
    write-output $_
    exit 1
}

function Get-TagFromMetadata {
    param(
            [Parameter(Position=1,Mandatory=1)]$metadata
        )

    process {
        $tagSplit = $metadata.version.Split('{+}')
        $tag = if($tagSplit[1] -ne $null) { $tagSplit[1] } else { 'master' }

        # Returns the tag in a format like MyTag or master if not specified for version 0.1.183-beta+MyTag
        return "$($tag)"
    }
}


function Get-CompatibleVersionFromMetadata {
    param(
            [Parameter(Position=1,Mandatory=1)]$metadata
        )

    process {
        $versionSplit = $metadata.version.Split('{-}')

        # Returns the version in a format like 0.1.183 instead of 0.1.183-beta+MyTag
        return "$($versionSplit[0])"
    }
}

$pacts = Get-ChildItem $pactsDir -Filter "*.json" -Recurse

if ($pacts.Length -eq 0) {
    throw "No pacts found in directory '$pactsDir'. Looked recursively for mask '*.json'."
}


$buildMetadataFilePath = Join-Path $workingDir $buildMetadataFile
$metadata = Get-Content $buildMetadataFilePath | Out-String | ConvertFrom-Json

$version = Get-CompatibleVersionFromMetadata $metadata
$tag = Get-TagFromMetadata $metadata

$pactContent = Get-Content $pacts[0].FullName | Out-String 
$pactJson = $pactContent | ConvertFrom-Json
$consumer = $pactJson.consumer.name
$encodedConsumer = [Uri]::EscapeDataString($consumer)

foreach($pact in $pacts){
    $content = Get-Content $pact.FullName | Out-String 
    $json = $content | ConvertFrom-Json
    
    $provider = $json.provider.name
    
    Write-Host "Publishing pact:"
    Write-Host "`t[$consumer => $provider], #$version"

    $encodedBranch = [Uri]::EscapeDataString(($tag -Replace "/", ""))
    $encodedProvider = [Uri]::EscapeDataString($provider)

    $url = (($templateUrl -Replace ":provider", $encodedProvider) -Replace ":consumer", $encodedConsumer) -Replace ":version", $version

    Write-Host "`t$url"
    Invoke-RestMethod $url -Body $content -Method "PUT" -Headers @{"Content-Type"= "application/json"}
}

$tagUrl = (($tagTemplateUrl -Replace ":consumer", $encodedConsumer) -Replace ":tag", $encodedBranch) -Replace ":version", $version

Write-Host "Tagging pact consumer $($consumer) with tag $($tag) for version $($version)"
Write-Host "`t$tagUrl"   

$headers =  @{
    "Content-Type"= "application/json" 
    "Accept"= "application/hal+json"
}

Invoke-RestMethod $tagUrl -Method "PUT" -Headers $headers