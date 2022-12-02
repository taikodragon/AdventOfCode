using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{

    [DayInfo(2020, 04, "Passport Processing")]
    class Day04 : ASolution
    {
        List<string> requiredFields = new List<string> { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
        List<string> validEyeColors = new List<string> { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
        List<string> lines;
        public Day04() : base(false)
        {
            

            lines = Input.Split("\n").ToList();
        }

        
        protected override string SolvePartOne()
        {
            int validCount = 0, ttl = 0;
            List<string> fieldsPresent = new List<string>();
            foreach(string line in lines) {
                if( line == string.Empty ) {
                    // Validate here
                    if( requiredFields.TrueForAll(f => fieldsPresent.Contains(f)) )
                        validCount++;

                    ttl++;
                    fieldsPresent.Clear();
                    continue;
                }
                // add to fields in badge
                fieldsPresent.AddRange(line.Split(' ').Select( kv => kv.Split(':') ).Select(kv => kv[0]));
            }
            if( fieldsPresent.Count != 0 ) {
                ttl++;
                if( requiredFields.TrueForAll(f => fieldsPresent.Contains(f)) )
                    validCount++;
            }
            return $"{validCount}/{ttl}";
        }

        bool IsValidPassport(Dictionary<string,string> fields)
        {
            if( !requiredFields.TrueForAll(f => fields.ContainsKey(f)) )
                return false;
            { // Birth Year
                if( !int.TryParse(fields["byr"], out int year) )
                    return false;
                if( year < 1920 || year > 2002 || fields["byr"].Length != 4 )
                    return false;
            }
            { // Issue Year
                if( !int.TryParse(fields["iyr"], out int year) )
                    return false;
                if( year < 2010 || year > 2020 || fields["iyr"].Length != 4 )
                    return false;
            }
            { // Expiration Year
                if( !int.TryParse(fields["eyr"], out int year) )
                    return false;
                if( year < 2020 || year > 2030 || fields["eyr"].Length != 4 )
                    return false;

            }
            { // Height
                string height = fields["hgt"];
                if( height.EndsWith("cm") ) {
                    if( !int.TryParse(height.TrimEnd('c', 'm'), out int val) )
                        return false;
                    if( val < 150 || val > 193 )
                        return false;
                } else if( height.EndsWith("in") ) {
                    if( !int.TryParse(height.TrimEnd('i', 'n'), out int val) )
                        return false;
                    if( val < 59 || val > 76 )
                        return false;
                } else
                    return false;
            }
            { // Hair Color
                if( !Regex.IsMatch(fields["hcl"], "^#[a-f0-9]{6}$") )
                    return false;
            }
            { // Eye Color
                if( !validEyeColors.Contains(fields["ecl"]) )
                    return false;
            }
            { // Passport Id
                if( !Regex.IsMatch(fields["pid"], "^[0-9]{9}$") )
                    return false;
            }
            //string output = $"byr:{fields["byr"]}\tiyr:{fields["iyr"]}\teyr:{fields["eyr"]}\thgt:{fields["hgt"]}\thcl:{fields["hcl"]}\tecl:{fields["ecl"]}\tpid:{fields["pid"]}";
            //Console.WriteLine(output);
            //Trace.WriteLine(output);
            //File.AppendAllText("debugOut",output+"\n");
            return true;
        }

        protected override string SolvePartTwo()
        {
            int validCount = 0, ttl = 0;
            Dictionary<string, string> fieldsPresent = new Dictionary<string, string>();
            foreach( string line in lines ) {
                if( line == string.Empty ) {
                    // Validate here
                    if( IsValidPassport(fieldsPresent) )
                        validCount++;

                    ttl++;
                    fieldsPresent.Clear();
                    continue;
                }
                // add to fields in badge
                line.Split(' ').Select(kv => kv.Split(':')).ToList().ForEach(kv => {
                    fieldsPresent.Add(kv[0], kv[1]);
                });
            }
            if( fieldsPresent.Count != 0 ) {
                ttl++;
                if( IsValidPassport(fieldsPresent) )
                    validCount++;
            }
            return $"{validCount}/{ttl}";
        }
    }
}
