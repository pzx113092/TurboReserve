using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TurboReserve.Models;
using TurboReserve.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TurboReserve.Controllers
{
    [Authorize(Roles = "Customer")]

    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(IReservationService reservationService, UserManager<IdentityUser> userManager)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var reservations = await _reservationService.GetByCustomerIdAsync(userId);
            return View(reservations);
        }


        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            
            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Customer") && reservation.CustomerId != userId)
            {
                return Forbid();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                
                var userId = _userManager.GetUserId(User);
                reservation.CustomerId = userId;
                reservation.Status = Models.Enums.ReservationStatus.Pending;
                await _reservationService.AddAsync(reservation);
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Customer") && reservation.CustomerId != userId)
            {
                return Forbid();
            }

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            var existingReservation = await _reservationService.GetByIdAsync(id);
            if (existingReservation == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Customer") && existingReservation.CustomerId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                
                existingReservation.ReservationDate = reservation.ReservationDate;
                existingReservation.Status = reservation.Status;
                existingReservation.ServiceId = reservation.ServiceId;
                existingReservation.ServiceProviderId = reservation.ServiceProviderId;
                await _reservationService.UpdateAsync(existingReservation);
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Customer") && reservation.CustomerId != userId)
            {
                return Forbid();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (User.IsInRole("Customer") && reservation.CustomerId != userId)
            {
                return Forbid();
            }

            await _reservationService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

