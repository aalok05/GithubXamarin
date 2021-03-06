﻿using System.Collections.Generic;
using Octokit;
using System.Threading.Tasks;

namespace GithubXamarin.Core.Contracts.Repository
{
    public interface IRepoRepository
    {
        Task<Octokit.Repository> GetRepository(long repositoryId, GitHubClient githubClient);
        Task<IEnumerable<Octokit.Repository>> GetAllRepositoriesForCurrentUser(GitHubClient authorizedGitHubClient);
        Task<IEnumerable<Octokit.Repository>> GetAllRepositoriesForUser(string login, GitHubClient gitHubClient);
        Task<IEnumerable<Octokit.Repository>> SearchRepositories(string searchTerm, GitHubClient githubClient);
        Task<Octokit.Repository> ForkRepository(long repositoryId, GitHubClient authorizedGithubClient);
        Task<bool> StarRepository(string repositoryOwner,string repositoryName, GitHubClient authorizedGithubClient);
        Task<bool> UnStarRepository(string repositoryOwner, string repositoryName, GitHubClient authorizedGithubClient);
    }
}