using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IReviewService : IBaseService<Review, int, VwFacilityReviewsDetail>
{
    Task AddAsyncReview(Review review);
}