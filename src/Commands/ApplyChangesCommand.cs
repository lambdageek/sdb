//
// The MIT License (MIT)
//
// Copyright (c) 2021 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System.IO;
using System.Linq;

namespace Mono.Debugger.Client.Commands
{
    sealed class ApplyChangesCommand : Command
    {
        public override string[] Names
        {
            get { return new[] { "apply-changes", }; }
        }

        public override string Summary
        {
            get { return "Apply a hot reload change to an assembly."; }
        }

        public override string Syntax
        {
            get { return "apply-changes module deltafile.dmeta deltafile.dil deltafile.dpdb "; }
        }

        public override string Help
        {
            get
            {
                return "Applies a hot reload change to an assembly.";
            }
        }

        public override void Process(string args_)
        {
	    var args = args_.Split(' ').Where(x => x != string.Empty).ToArray ();
	    if (args.Length < 3 ||  args.Length > 4) {
		Log.Error ("need a module, and dmeta and dil files");
		return;
	    }
	    var moduleName = args[0];
	    var dmetaName = args[1];
	    var dilName = args[2];
	    var dpdbName = args.Length == 4 ? args[3] : null;

	    if (!File.Exists (dmetaName)) {
		Log.Error ($"module metadata delta '{dmetaName}' doesn't exist");
		return;
	    }

	    if (!File.Exists (dilName)) {
		Log.Error ($"module IL delta '{dilName}' doesn't exist");
		return;
	    }
	    
	    if (dpdbName != null && !File.Exists (dpdbName)) {
		Log.Error ($"module PDB delta '{dpdbName}' doesn't exist");
		return;
	    }

	    var dmetaBytes = File.ReadAllBytes (dmetaName);
	    var dilBytes = File.ReadAllBytes (dilName);
	    var dpdbBytes = dpdbName != null ? File.ReadAllBytes (dpdbName) : null;

	    Debugger.ApplyChanges (moduleName, dmetaBytes, dilBytes, dpdbBytes);

        }
    }
}
