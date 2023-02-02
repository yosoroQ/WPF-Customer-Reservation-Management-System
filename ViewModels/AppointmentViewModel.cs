using System;
using _05SQL.Models;

namespace WPF_CMS.ViewModels
{
    public class AppointmentViewModel
    {
        private Appointment _appointment;
        public AppointmentViewModel(Appointment appointment)
        {
            _appointment = appointment;
        }

        public int Id { get => _appointment.Id;  }
        public DateTime Time { get => _appointment.Time;
            set
            {
                if (value != _appointment.Time)
                {
                    _appointment.Time = value;
                }
            }}
    }
}