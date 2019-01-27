using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitParser
{
    public class GitCommands
    {
        internal static string ListShaWithFiles(string path)
        {
            var output = RunProcess(string.Format(" --git-dir={0}/.git --work-tree={1} log --name-status", path.Replace("\\", "/"), path.Replace("\\", "/")));
            return output;
        }

        private bool StartsWithHeader(string line)
        {
            if (line.Length > 0 && char.IsLetter(line[0]))
            {
                var seq = line.SkipWhile(ch => Char.IsLetter(ch) && ch != ':');
                return seq.FirstOrDefault() == ':';
            }
            return false;
        }

        internal List<GitCommit> Parse(string output)
        {
            GitCommit commit = null;
            var commits = new List<GitCommit>();
            bool processingMessage = false;
            using (var strReader = new StringReader(output))
            {
                do
                {
                    var line = strReader.ReadLine();

                    if (line.StartsWith("commit "))
                    {
                        if (commit != null)
                            commits.Add(commit);
                        commit = new GitCommit();
                        commit.Sha = line.Split(' ')[1];
                    }

                    if (StartsWithHeader(line))
                    {
                        var header = line.Split(':')[0];
                        var val = string.Join(":", line.Split(':').Skip(1)).Trim();

                        // headers
                        commit.Headers.Add(header, val);
                    }

                    if (string.IsNullOrEmpty(line))
                    {
                        // commit message divider
                        processingMessage = !processingMessage;
                    }

                    if (line.Length > 0 && line[0] == '\t')
                    {
                        // commit message.
                        commit.Message += line;
                    }

                    if (line.Length > 1 && Char.IsLetter(line[0]) && line[1] == '\t')
                    {
                        var status = line.Split('\t')[0];
                        var file = line.Split('\t')[1];
                        commit.Files.Add(new GitFileStatus() { Status = status, File = file });
                    }
                }
                while (strReader.Peek() != -1);
            }

            if (commit != null)
                commits.Add(commit);

            return commits;
        }

        private static string RunProcess(string command)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "git.exe";/*Config.GitExectuable;*/
            p.StartInfo.Arguments = command;
            p.Start();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        public List<GitCommit> DoRun(string[] args)
        {
            string path = String.Empty;
            if (args.Length > 0)
                path = args[0];

            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Repository path not found!");
            }

            var output = GitCommands.ListShaWithFiles(path);

            return  Parse(output);
        }
    }
}
