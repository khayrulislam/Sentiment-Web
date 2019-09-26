using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class RepositoryService
    {

        public async Task ExecuteRepositoryAnalysisAsync(string repositoryUrl)
        {

            // get repository Information

            var productInformation = new ProductHeaderValue("test-app");

            var client = new GitHubClient(productInformation) {
                Credentials = credential
            };

            var repository = await client.Repository.Get("mockito", "mockito");

            

            var x = repository.Owner;

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                //var user = unitOfWork.User.Get(1);

                var user = new UserData();

                user.FirstName = "olife";
                user.LastName = "olife";
                user.Email = "abc@gmail.com";
                user.Password = "123";


                unitOfWork.User.Add(user);
                unitOfWork.Complete();

                var repo = new RepositoryData()
                {
                    Name = repository.Name,
                    User  =  user,
                    OwnerName = repository.Owner.Name,
                    Url =repository.Url,
                    UserId= user.Id
                };

                unitOfWork.Repository.Add(repo);
                unitOfWork.Complete();

            }

        }

    }
}
