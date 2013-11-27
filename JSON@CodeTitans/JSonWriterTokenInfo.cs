#region License
/*
    Copyright (c) 2010, Paweł Hofman (CodeTitans)
    All Rights Reserved.

    Licensed under the Apache License version 2.0.
    For more information please visit:

    http://codetitans.codeplex.com/license
        or
    http://www.apache.org/licenses/


    For latest source code, documentation, samples
    and more information please visit:

    http://codetitans.codeplex.com/
*/
#endregion

using System.IO;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Internal class used by <see cref="JSonWriter"/> to keep the status of current writing progress.
    /// </summary>
    internal sealed class JSonWriterTokenInfo
    {
        public JSonWriterTokenInfo(JSonWriterTokenType tokenType, int level)
        {
            TokenType = tokenType;
            Level = level;
            ItemsCount = 0;
        }

        public JSonWriterTokenType TokenType
        { get; private set; }

        public int Level
        { get; private set; }

        public int ItemsCount
        { get; private set; }

        public void IncreaseItemCount()
        {
            ItemsCount = ItemsCount + 1;
        }

        public bool AddItem(TextWriter output, bool indent, bool compactEnumerables)
        {
            ItemsCount++;
            if (ItemsCount > 1)
            {
                if (indent)
                {
                    output.WriteLine(',');
                    if (Level > 0)
                        output.Write(new string(' ', Level * 4));
                }
                else
                {
                    if (compactEnumerables)
                        output.Write(',');
                    else
                        output.Write(", ");
                    return true;
                }

                return false;
            }

            if (indent)
            {
                output.WriteLine();
                if (Level > 0)
                    output.Write(new string(' ', Level * 4));
            }

            return false;
        }

        public void RemoveItem(TextWriter output, bool indent)
        {
            if (indent)
            {
                if (ItemsCount > 0)
                {
                    output.WriteLine();
                    if (Level > 1)
                        output.Write(new string(' ', Level * 4 - 4));
                }
            }
        }
    }
}
