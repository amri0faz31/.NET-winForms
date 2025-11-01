using System.Collections.Generic;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;
using samp_01.Domain.Entities;

namespace samp_01.Services.ServiceProvider
{
    //service for managing seller portfolios and projects
    public class PortfolioService
    {
        private readonly IPortfolioRepository _repo;
        public PortfolioService() : this(new AdoPortfolioRepository(AppConfig.ConnectionString)) { }
        public PortfolioService(IPortfolioRepository repo) { _repo = repo; }

        public SellerPortfolioDTO? Get(int sellerId) => _repo.GetPortfolio(sellerId);
        //save or update seller portfolio
        public void Save(int sellerId, string? description, decimal? min, decimal? max, string? skills, string? profilePicPath)
        {
            var entity = new SellerPortfolio
            {
                SellerId = sellerId,
                Description = description,
                PriceRangeMin = min,
                PriceRangeMax = max,
                Skills = skills,
                ProfilePicPath = profilePicPath
            };
            _repo.UpsertPortfolio(entity);
        }
        //list all projects for a given seller
        public List<SellerProjectDTO> ListProjects(int sellerId) => _repo.GetProjects(sellerId);
        //add a new project to the seller's portfolio
        public int AddProject(int sellerId, string title, string? description, string? imagePath)
        {
            var p = new SellerProject { SellerId = sellerId, Title = title, Description = description, ImagePath = imagePath };
            return _repo.AddProject(p);
        }
        //delete a project from the seller's portfolio
        public bool DeleteProject(int projectId, int sellerId) => _repo.DeleteProject(projectId, sellerId);
    }
}
