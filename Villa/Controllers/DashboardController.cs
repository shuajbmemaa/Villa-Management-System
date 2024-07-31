using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Infrastructure.Repository;
using Villa.ViewModels;

namespace Villa.Controllers
{
    public class DashboardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month-1; //psh nese osht Janar(Janari o 1) 1-1 = 0 kshtuqe bone 12 Dhjetor ose -1muj
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);


        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != Const.StatusPending
            || u.Status != Const.StatusCancelled);

            var countByCurrentMonth=totalBookings.Count(u=>u.BookingDate >= currentMonthStartDate &&
             u.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate &&
             u.BookingDate <= currentMonthStartDate);

            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;

            if(countByPreviousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((countByCurrentMonth-countByPreviousMonth)/countByPreviousMonth*100);
            }

            radialBarChartVM.TotalC = totalBookings.Count();
            radialBarChartVM.CountInCurrentMonth = countByCurrentMonth;
            radialBarChartVM.IsIncreased = currentMonthStartDate > previousMonthStartDate;
            radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

            return Json(radialBarChartVM);


        }


    }
}
