using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Ui.Core
{
    public static class DefinedSymbols
    {
        public static ReadOnlyCollection<string> Symbols { get; private set; }
        internal static string[] SymbolsInternal { get; private set; }

        public static bool HasSymbol(string symbol)
        {
            return SymbolsInternal.Contains(symbol);
        }

        public static void Define(string symbol)
        {
            List<string> symbols = SymbolsInternal != null ? SymbolsInternal.ToList() : new List<string>();

            if (!symbols.Contains(symbol))
            {
                symbols.Add(symbol);
            }

            SymbolsInternal = symbols.ToArray();
            Symbols = new ReadOnlyCollection<string>(SymbolsInternal);
        }

        public static void Undefine(string symbol)
        {
            List<string> symbols = SymbolsInternal.ToList();
            symbols.Remove(symbol);

            SymbolsInternal = symbols.ToArray();
            Symbols = new ReadOnlyCollection<string>(SymbolsInternal);
        }
    }
}
