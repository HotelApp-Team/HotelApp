using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Admin panelinden rezervasyon durumunu genel olarak değiştirmek için kullanılır.
    public class AdminUpdateReservationStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    // Admin panelindeki hızlı onay / reddet akışı için kullanılır.
    public class AdminConfirmReservationDto
    {
        public bool Confirm { get; set; }
    }
}
