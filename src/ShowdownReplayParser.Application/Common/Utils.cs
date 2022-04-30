using ShowdownReplayParser.Application.Models.Exceptions;

namespace ShowdownReplayParser.Application.Common
{
    public static class Utils
    {
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            throw new UtilsException("Could not find words provided in string");
        }

        public static string FindNext(string source, string wordToFind)
        {
            for(int i = 0; i < source.Length; i++)
            {
                if(i+wordToFind.Length > source.Length)
                {
                    throw new UtilsException("Word not found");
                }

                if(source.Substring(i, wordToFind.Length) == wordToFind)
                {
                    return source.Substring(i + wordToFind.Length, source.Length);
                }
            }

            throw new UtilsException("Word not found");
        }

        public static int FindNextIndex(string source, string wordToFind)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (i + wordToFind.Length > source.Length)
                {
                    throw new UtilsException("Word not found");
                }

                if (source.Substring(i, wordToFind.Length) == wordToFind)
                {
                    return i;
                }
            }

            throw new UtilsException("Word not found");
        }
    }
}
