using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class ReviewService : BaseService<Review, int, VwFacilityReviewsDetail>, IReviewService
{
    public ReviewService(IBaseRepository<Review, int, VwFacilityReviewsDetail> repository) : base(repository)
    {
    }

    public async Task AddAsyncReview(Review review)
    {
        await AddAsync(review);
        await SaveChangesAsync("system", false);
    }
}