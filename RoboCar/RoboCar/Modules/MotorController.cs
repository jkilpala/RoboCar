//////////////////////////////////////////////////////////////////////////////////////////////////
///   Rotationally  |   IN1    |    IN 2        IN3         IN4
///                 |          |
///     Forward     |   H      |    L           -           -
/// M1  Reversion   |   L      |    H           -           -
///     Stop        |   L      |    L           -           -
///
///     Forward     |   -      |    -           H           L
/// M2  Reversion   |   -      |    -           L           H
///     Stop        |   -      |    -           L           L
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using Windows.Devices.Enumeration;
using RoboCar.Modules;
using RoboCar.Data;



namespace RoboCar
{    
    /// <summary>
    /// Arduino controller using firamata
    /// </summary>
    class MotorController
    {
        IStream connection;
        RemoteDevice arduino;
        bool _initialized;
        //Left Motor
        byte Motor1A = 4;
        byte Motor1B = 5;
        byte Motor1PWM = 9;
        //Right Motor
        byte Motor2A = 6;
        byte Motor2B = 7;
        byte Motor2PWM = 10;

        public void SetupRemoteArduino(DeviceInformation device)
        {
            connection = new UsbSerial(device);
            arduino = new RemoteDevice(connection);

            arduino.DeviceReady += Arduino_DeviceReady;

            connection.begin(57600, SerialConfig.SERIAL_8N1);
            
        }
        public void SetupRemoteArduino()
        {
            connection = new UsbSerial(SerialDevices.Instance.GetUSBDevice());
            arduino = new RemoteDevice(connection);

            arduino.DeviceReady += Arduino_DeviceReady;

            connection.begin(57600, SerialConfig.SERIAL_8N1);

        }
        private void Arduino_DeviceReady()
        {
            System.Diagnostics.Debug.WriteLine("Arduino device ready...");
            Setup();
            UnityClient.Instance.MoveDataChanged += Instance_MoveDataChanged;                  
        }

        private void Instance_MoveDataChanged(Data.MoveData md)
        {
            System.Diagnostics.Debug.WriteLine("MotorController::MoveDataChanged::Do some turning and stuff");
            WriteMovement(md);
        }

        private void WriteMovement(MoveData md)
        {
            if (!_initialized)
            {
                return;
            }
            //Replace with just one call with md values
            //if (md.Left && md.Right)
            //{
            //    arduino.digitalWrite(Motor1A, PinState.HIGH);
            //    arduino.digitalWrite(Motor1B, PinState.HIGH);

            //    arduino.digitalWrite(Motor2B, PinState.HIGH);
            //    arduino.digitalWrite(Motor2A, PinState.HIGH);

            //    //analog write between 0 - 255 convert to 0 - 1
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.LeftValue));
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.RightValue));
            //}
            //else if (!md.Left && !md.Right)
            //{
            //    arduino.digitalWrite(Motor1A, PinState.LOW);
            //    arduino.digitalWrite(Motor1B, PinState.LOW);

            //    arduino.digitalWrite(Motor2B, PinState.LOW);
            //    arduino.digitalWrite(Motor2A, PinState.LOW);

            //    //analog write between 0 - 255 convert to 0 - 1
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.LeftValue));
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.RightValue));
            //}
            //else if (!md.Left && md.Right)
            //{
            //    arduino.digitalWrite(Motor1A, PinState.HIGH);
            //    arduino.digitalWrite(Motor1B, PinState.HIGH);

            //    arduino.digitalWrite(Motor2B, PinState.LOW);
            //    arduino.digitalWrite(Motor2A, PinState.LOW);

            //    //analog write between 0 - 255 convert to 0 - 1
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.LeftValue));
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.RightValue));
            //}
            //if (md.Left && !md.Right)
            //{
            //    arduino.digitalWrite(Motor1A, PinState.HIGH);
            //    arduino.digitalWrite(Motor1B, PinState.HIGH);

            //    arduino.digitalWrite(Motor2B, PinState.HIGH);
            //    arduino.digitalWrite(Motor2A, PinState.HIGH);

            //    //analog write between 0 - 255 convert to 0 - 1
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.LeftValue));
            //    arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.RightValue));
            //}
            ResetStateBeforeChange();

            arduino.digitalWrite(Motor1A, ConvertBoolToPinState(md.Left));
            arduino.digitalWrite(Motor1B, ConvertBoolToPinState(!md.Left));

            
            arduino.digitalWrite(Motor2A, ConvertBoolToPinState(md.Right));
            arduino.digitalWrite(Motor2B, ConvertBoolToPinState(!md.Right));

            //analog write between 0 - 255 convert to 0 - 1
            arduino.analogWrite(Motor1PWM, ConvertFloatToUshort(md.LeftValue));
            arduino.analogWrite(Motor2PWM, ConvertFloatToUshort(md.RightValue));


        }
        private void ResetStateBeforeChange()
        {
            arduino.digitalWrite(Motor1A, 0);
            arduino.digitalWrite(Motor1B, 0);

            arduino.digitalWrite(Motor2B, 0);
            arduino.digitalWrite(Motor2A, 0);

            //analog write between 0 - 255 convert to 0 - 1
            arduino.analogWrite(Motor1PWM, 0);
            arduino.analogWrite(Motor2PWM, 0);
        }
        PinState ConvertBoolToPinState(bool value)
        {
            if (value)
            {
                return PinState.HIGH;
            }
            return PinState.LOW;
        }
        ushort ConvertFloatToUshort(float value)
        {
            return (ushort)(255 * value);
        }
        private void Setup()
        {
            //arduino.pinMode(9, PinMode.OUTPUT);
            arduino.pinMode(Motor1A, PinMode.OUTPUT);
            arduino.pinMode(Motor1B, PinMode.OUTPUT);
            arduino.pinMode(Motor1PWM, PinMode.PWM);

            arduino.pinMode(Motor2A, PinMode.OUTPUT);
            arduino.pinMode(Motor2B, PinMode.OUTPUT);
            arduino.pinMode(Motor2PWM, PinMode.PWM);

            ResetStateBeforeChange();
            _initialized = true;
        }
        PinState currentState = PinState.LOW;
        public void WriteToPin()
        {
            if (_initialized)
            {
                System.Diagnostics.Debug.WriteLine("Arduino Led!");
                if (currentState == PinState.LOW)
                {
                    currentState = PinState.HIGH;
                }
                else
                {
                    currentState = PinState.LOW;
                }
                arduino.digitalWrite(9, currentState); 
            }
        }
    }
}
