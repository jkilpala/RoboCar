using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI.Core;


namespace RoboCar
{
    /// <summary>
    /// XboxOne Controller Module
    /// </summary>
    class BoxController
    {
        private Gamepad _gamePad = null;       
        CoreDispatcher dispacher;
        public void SetUp()
        {
           // var controller = Gamepad.Gamepads.First();
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
            
        }

        private async Task GetInput()
        {
            while (true)
            {
                await dispacher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        if (_gamePad == null)
                        {
                            return;
                        }
                        var reading = _gamePad.GetCurrentReading();

                        if (reading.LeftTrigger >= 0)
                        {
                            System.Diagnostics.Debug.WriteLine("LT:" + reading.LeftTrigger);
                        }
                        if (reading.RightTrigger >= 0)
                        {
                            System.Diagnostics.Debug.WriteLine("RT:" + reading.RightTrigger);
                        }
                    });
            }
        }
        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            _gamePad = null;
        }

        private async void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            _gamePad = Gamepad.Gamepads.First();
            await GetInput();
        }
    }
}
