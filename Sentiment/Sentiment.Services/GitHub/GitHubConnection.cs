using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.GitHub
{
    public sealed class GitHubConnection
    {
        private GitHubConnection()
        {
        }
        private static readonly object padlock = new object();

        private static GitHubClient instance = null;
        public static GitHubClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = GetGitHubClient();
                        }
                    }
                }
                return instance;
            }
        }

        private static GitHubClient GetGitHubClient()
        {
            var productInformation = new ProductHeaderValue("test-app");
            var credential = new Credentials("khayrulislam", "ki10091997");

            var client = new GitHubClient(productInformation)
            {
                Credentials = credential
            };
            return client;
        }
    }
}
