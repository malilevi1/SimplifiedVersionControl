using System;
using System.Collections.Generic;
using System.Text;

namespace GitParser
{
    public class GitCommit
    {
        public GitCommit()
        {
            Headers = new Dictionary<string, string>();
            Files = new List<GitFileStatus>();
            Message = String.Empty;
        }

        public Dictionary<string, string> Headers { get; set; }
        public string Sha { get; set; }
        public string Message { get; set; }
        public List<GitFileStatus> Files { get; set; }

        public string Print()
        {
            var sb = new StringBuilder();
            Files.ForEach(e => sb.Append($"{e.Status}\t{e.File}"));

            return $"commit: {Sha} {GetHeader()} {Environment.NewLine} {Message} {Environment.NewLine} {sb}";
        }

        public override string ToString()
            => GetHeader();

        public string GetHeader()
        {
            var sbHeader = new StringBuilder();

            foreach (var key in Headers.Keys)
            {
                sbHeader.Append(key + ":" + Headers[key]);
            }

            return sbHeader.ToString();
        }
    }
}
