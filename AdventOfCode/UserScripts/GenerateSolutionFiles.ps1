param(
    [int]$Year = (Get-Date).Year
)

$template = @"
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year<YEAR>;

[DayInfo(<YEAR>, <DAY>, `"`")]
class Day<DAY> : ASolution
{

    public Day<DAY>() : base(false)
    {
            
    }

    protected override void ParseInput()
    {

    }

    protected override object SolvePartOneRaw()
    {
        return null;
    }

    protected override object SolvePartTwoRaw()
    {
        return null;
    }
}
"@

$newDirectory = Join-Path $PSScriptRoot "../Solutions/Year$Year"
Write-Host "Generating for $newDirectory"
if(!(Test-Path $newDirectory)) {
    New-Item $newDirectory -ItemType Directory | Out-Null
}

for($i = 1; $i -le 25; $i++) {
    $newFile = Join-Path $newDirectory "Day$("{0:00}" -f $i)-Solution.cs"
    if(!(Test-Path $newFile)) {
        Write-Host $newFile
        New-Item $newFile -ItemType File -Value ($template -replace "<YEAR>", $Year -replace "<DAY>", "$("{0:00}" -f $i)") -Force | Out-Null
    }
}
for($i = 1; $i -le 25; $i++) {
    $newFile = Join-Path $newDirectory "Day$("{0:00}" -f $i)-debugInput"
    if(!(Test-Path $newFile)) {
        Write-Host $newFile
        New-Item $newFile -ItemType File -Value "" -Force | Out-Null
    }
}

Write-Host "Files Generated"
