﻿param(
    [int]$Year = (Get-Date).Year
)

$template = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year<YEAR>
{

    class Day<DAY> : ASolution
    {

        public Day<DAY>() : base(<DAY>, <YEAR>, `"`")
        {
            UseDebugInput = false;
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}

"@

$newDirectory = Join-Path $PSScriptRoot ".." "Solutions" "Year$Year" -Resolve
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

Write-Host "Files Generated"
