using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2015
{
    class Day07 : ASolution
    {
        class LogicGate
        {
            string[]? myRequiredSignals;
            public ushort? LHSValue { get; private set; }
            public string? LHS { get; private set; }
            public ushort? RHSValue { get; private set; }
            public string? RHS { get; private set; }
            public string? Operator { get; set; }
            public string? OutputTo { get; set; }

            public void SetLHS(string value) {
                if( ushort.TryParse(value, out ushort val) ) {
                    LHSValue = val;
                    RHS = null;
                }
                else {
                    LHSValue = null;
                    LHS = value;
                }
            }
            public void SetRHS(string value) {
                if( ushort.TryParse(value, out ushort val) ) {
                    RHSValue = val;
                    RHS = null;
                }
                else {
                    RHSValue = null;
                    RHS = value;
                }
            }
            public bool Simulate(Dictionary<string,ushort> signals, Dictionary<string,List<LogicGate>> related, List<LogicGate>? parentSims = null) {
                if( parentSims?.Contains(this) == true ) throw new Exception("Loop detected!");
                if( signals.ContainsKey(OutputTo!) ) return true;
                foreach(string depSignal in RequiresSignals()) {
                    if( !signals.ContainsKey(depSignal) ) return false;
                }
                ushort lhs, rhs;
                switch(Operator) {
                    default: return true;
                    case "AND":
                        lhs = LHSValue ?? signals[LHS];
                        rhs = RHSValue ?? signals[RHS!];
                        signals[OutputTo!] = (ushort)(lhs & rhs);
                        break;
                    case "OR":
                        lhs = LHSValue ?? signals[LHS];
                        rhs = RHSValue ?? signals[RHS!];
                        signals[OutputTo!] = (ushort)(lhs | rhs);
                        break;
                    case "LSHIFT":
                        lhs = LHSValue ?? signals[LHS];
                        rhs = RHSValue ?? signals[RHS!];
                        signals[OutputTo!] = (ushort)(lhs << rhs);
                        break;
                    case "RSHIFT":
                        lhs = LHSValue ?? signals[LHS];
                        rhs = RHSValue ?? signals[RHS!];
                        signals[OutputTo!] = (ushort)(lhs >> rhs);
                        break;
                    case "NOT":
                        rhs = RHSValue ?? signals[RHS!];
                        signals[OutputTo!] = (ushort)(~rhs);
                        break;
                    case "COPY":
                        signals[OutputTo!] = RHSValue ?? signals[RHS!];
                        break;
                }
                if( related.TryGetValue(OutputTo!, out List<LogicGate>? otherGates) ) {
                    parentSims ??= new List<LogicGate>();
                    parentSims.Add(this);
                    foreach( var gate in otherGates ) {
                        if( gate == this ) continue;
                        gate.Simulate(signals, related, parentSims);
                    }
                    parentSims.Remove(this);
                }
                return true;
            }
            public string[] RequiresSignals() {
                if( myRequiredSignals != null ) return myRequiredSignals;
                List<string> result = new List<string>();
                switch( Operator ) {
                    default: return Array.Empty<string>();
                    case "AND":
                    case "OR":
                    case "LSHIFT":
                    case "RSHIFT":
                        if( !LHSValue.HasValue ) result.Add(LHS);
                        if( !RHSValue.HasValue ) result.Add(RHS);
                        break;
                    case "NOT":
                    case "COPY":
                        if( !RHSValue.HasValue ) result.Add(RHS);
                        break;
                }
                return myRequiredSignals = result.ToArray();
            }
        }
        List<LogicGate> rootGates = new List<LogicGate>();
        Dictionary<string, List<LogicGate>> relatedGates = new Dictionary<string, List<LogicGate>>();
        public Day07() : base(07, 2015, "Some Assembly Required", false)
        {
            UseDebugInput = true;

            foreach(string line in Input.SplitByNewline()) {
                string[] lineParts = line.Split("->").Select(p => p.Trim()).ToArray();
                string[] gateParts = lineParts[0].Split(' ');

                LogicGate lg = new LogicGate() {
                    OutputTo = lineParts[1]
                };                    ;
                switch( gateParts.Length ) {
                    default: continue;
                    case 1:
                        lg.Operator = "COPY";
                        lg.SetRHS(gateParts[0]);
                        break;
                    case 2:
                        lg.Operator = gateParts[0];
                        lg.SetRHS(gateParts[1]);
                        break;
                    case 3:
                        lg.SetLHS(gateParts[0]);
                        lg.Operator = gateParts[1];
                        lg.SetRHS(gateParts[2]);
                        break;
                }
                var deps = lg.RequiresSignals();
                if( deps.Length == 0 ) {
                    rootGates.Add(lg);
                }
                foreach(string signalName in deps) {
                    List<LogicGate> gates;
                    if( !relatedGates.TryGetValue(signalName, out gates) ) {
                        relatedGates.Add(signalName, gates = new List<LogicGate>());
                    }
                    gates.Add(lg);
                }
            }
        }


        protected override string SolvePartOne() {
            Dictionary<string, ushort> signals = new Dictionary<string, ushort>();
            foreach(var gate in rootGates) {
                gate.Simulate(signals, relatedGates); 
            }

            return signals["a"].ToString();
        }

        protected override string SolvePartTwo()
        {
            rootGates.First(lg => lg.OutputTo == "b").SetRHS(Part1);

            Dictionary<string, ushort> signals = new Dictionary<string, ushort>();
            foreach( var gate in rootGates ) {
                gate.Simulate(signals, relatedGates);
            }

            return signals["a"].ToString();
        }
    }
}
