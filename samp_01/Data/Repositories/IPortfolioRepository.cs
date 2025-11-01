using System.Collections.Generic;
using samp_01.Domain.DTO;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    public interface IPortfolioRepository
    {
        SellerPortfolioDTO? GetPortfolio(int sellerId);
        void UpsertPortfolio(SellerPortfolio p);
        List<SellerProjectDTO> GetProjects(int sellerId);
        int AddProject(SellerProject p);
        bool DeleteProject(int projectId, int sellerId);
    }
}
