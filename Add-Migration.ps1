param(
    [string]
    $name = $(Read-Host "Enter migration name")
)

$name = ((,$name + $args) -join "_") -replace "\s+", "_"

$name

$rootDir = resolve-path .
$migrationsDir = "$rootDir\database\migrations"

$version = gci -path "$migrationsDir" -Filter *.sql | % { $_.Name -replace "^(\d+).*?$","`$1" } | % { [int]$_ } | sort -Descending | select -First 1
$version++
$version = $version.ToString("D4")

$oldName = $name
$name = $name -replace " ", "_"

$newFileName = "$($version)_$name.sql"
$filePath = "$migrationsDir\$newFileName"

"-- $(Get-Date -Format "yyyy-MM-dd HH:mm") : $oldName" | set-content -Path "$filePath"

write-host "$filePath"