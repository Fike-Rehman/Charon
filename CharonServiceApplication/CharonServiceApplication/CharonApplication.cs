﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CTS.Charon.Devices;
using CTS.Common.Utilities;


namespace CTS.Charon.CharonApplication
{
    internal class CharonApplication
    {
        private static bool _consoleMode;

        private static readonly log4net.ILog _logger =
                 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Timer _pingtimer;
        private static Timer _changeStateR1Timer;
        private static Timer _changeStateR2Timer;

        // TODO: inject this as dependency
        private readonly NetDuinoPlus _netDuino;

       
        private static int _DCBusOnTimeOffset;
        private static DateTime _DCBusOffTime;
        private static int _ACBusOnTimeOffset;
        private static DateTime _ACBusOffTime;


        // TODO: re-factor this to smaller method
        public CharonApplication(bool consoleMode)
        {
            _consoleMode = consoleMode;

            if (_consoleMode)
            {
                Console.WriteLine($"Started Charon Service in console mode {DateTime.Now}");
                Console.WriteLine("Press any key to exit...");
                Console.WriteLine();
            }   
            else
                _logger.Info($"Started Charon Service in console mode {DateTime.Now}");


            // Read in the configurtion:
            var deviceIP = string.Empty;

            try
            {
               deviceIP = ConfigurationManager.AppSettings["deviceIPAddress"];

               _DCBusOnTimeOffset = Convert.ToInt16(ConfigurationManager.AppSettings["DCRelayOnTimeOffest"]);
               _DCBusOffTime = Convert.ToDateTime(ConfigurationManager.AppSettings["DCRelayOffTime"]);

               _ACBusOnTimeOffset = Convert.ToInt16(ConfigurationManager.AppSettings["DCRelayOnTimeOffest"]);
               _ACBusOffTime = Convert.ToDateTime(ConfigurationManager.AppSettings["ACRelayOffTime"]);
            }
            catch (ConfigurationErrorsException)
            {
                LogMessage("Error Reading Configuration File...");
            }

            // Initialize and execute a device Ping to see if our board is online:
            _netDuino = NetDuinoPlus.Instance(deviceIP);

            if (_netDuino.ExecutePing(LogMessage))
            {
                LogMessage("Device Initialization Success...");

                // Device initialization succeeded. We can continue with more operations:
                // set up a timer that sends a ping asynchronously every minute:
                var pingInterval = new TimeSpan(0, 0, 1, 0); // 1 minute  
                _pingtimer = new Timer(OnPingTimer, null, pingInterval, Timeout.InfiniteTimeSpan);

                // Start with setting up the netDuino relays:
               SetNetDuinoRelaysAsync();
            }
            else
            {
                // introduce a delay to give it a chance to report the progress:
                Thread.Sleep(1000);

                LogMessage($"Device Ping Failed after {_netDuino.NumTries} attempts");

                // There is not much point in continuing on at this point. Just send
                // out an Alert and stop the app:
                LogMessage("Device is either not online or has mal-functioned.");
                LogMessage("Sending Alert...");

                var alert = new  AlertSender();

                var @address = NetDuinoPlus.DeviceIPAddress.Substring(7, 13);

                var msg =
                    "Your device has failed to respond to Ping request(s) dispatched to address: " + @address + " after repeated attempts.\r\n" +
                    $"{Environment.NewLine}Event Date & Time: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()} {Environment.NewLine}" +
                    $"{Environment.NewLine}Please check device and make sure that it is still online!";

                LogMessage(alert.SendEmailAlert("Atert: Device Ping Failed", bodyText: msg)
                    ? "Alert dispatch via Email completed successfully"
                    : "Attempt to send an email alert failed!");

                LogMessage(alert.SendSMSAlert("Atert: Device Ping Failed", msg)
                    ? "Alert dispatch via SMS completed successfully"
                    : "Attempt to send an SMS alert failed!");
            }


            if (!_consoleMode)
            {
                Stop();
                return;
            }
            
            Console.ReadKey();
            Stop();
        }

        private static async void SetNetDuinoRelaysAsync()
        {
            await SetNetDuinoDCRelay();
            await SetNetDuinoACRelay();
        }

        /// <summary>
        /// Sends commands to set the current state of the NetDuino Relay R1 (DC Relay) based on the 
        /// configured OnTime and OffTime values
        /// </summary>
        /// <returns> returns a TimeSpan that tells us when we need to call this method again</returns>
        private static async Task<TimeSpan> SetNetDuinoDCRelay()
        {
            string result;
            var alert = new AlertSender();

            // First calulate the DC Bus On Time value using Today's Sunset time &
            // given on Time offset value:
            DateTime sunriseToday, sunsetToday;
            SunTimes.GetSunTimes(out sunriseToday, out sunsetToday);

            var onTime = sunsetToday - new TimeSpan(0, 0, _DCBusOnTimeOffset, 0);
            var offTime = _DCBusOffTime;

            if (onTime < offTime)
            {
                LogMessage("Invalid Configuration!. Please check the On/Off Time values");
                return TimeSpan.MinValue;
            }

            // time to wait for next state change trigger
            TimeSpan stateChangeInterval;
            
            if(DateTime.Now.TimeOfDay < onTime.TimeOfDay)
            {
                // we are in daytime
                // energize the relay 1 to turn lights off and set the interval for next state change
                result = await NetDuinoPlus.EnergizeRelay1();
                stateChangeInterval = onTime.TimeOfDay - DateTime.Now.TimeOfDay;
            }
            else if (DateTime.Now.TimeOfDay >= onTime.TimeOfDay && DateTime.Now.TimeOfDay <= offTime.TimeOfDay)
            {
                // we are in the onTime..
                // de-energize relay1 to turn the lights on and set them to turn off at offTime
                result = await NetDuinoPlus.DenergizeRelay1();
                stateChangeInterval = offTime.TimeOfDay - DateTime.Now.TimeOfDay;
                
                alert.SendSMSAlert("Alert",
                    $"DC Bus powered on at {onTime.ToLongTimeString()}. Today's Sunset Time: {sunsetToday.ToLongTimeString()}");
            }
            else
            {
                // Current time is between OffTime and midnight
                // energize the relays to turn the light off and set the interval to onTime + 1 Day
                result = await NetDuinoPlus.EnergizeRelay1();
                stateChangeInterval = (new TimeSpan(1,0,0,0) + onTime.TimeOfDay) - DateTime.Now.TimeOfDay;
            }

            if (result == "Success")
            {
                if (_changeStateR1Timer == null)
                {
                    //This is the first time this method is executed
                    // set up the timer to trigger next time the Relay state change is needed: 
                    _changeStateR1Timer = new Timer(OnChangeStateR1Timer, null, stateChangeInterval, Timeout.InfiniteTimeSpan);
                }
            }
            else
            {
                // here we deal with the failure...
                // set the TimeSpan to min value to return an indication of failure and log appropriate messages and alerts
                stateChangeInterval = TimeSpan.MinValue;
                
                var @address = NetDuinoPlus.DeviceIPAddress.Substring(7, 13);

                var msg =
                    "Netduino has failed to respond to Energize/Denergize DC relay request(s) dispatched to address: " + @address + "in a timely fashion" +
                    $"{Environment.NewLine}Event Date & Time: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()} {Environment.NewLine}" +
                    $"{Environment.NewLine}Please check Netduino and make sure that it is still online!";

                LogMessage(alert.SendEmailAlert("Atert: Energize/Denergize Relay request to NetDuino Failed", bodyText: msg)
                    ? "Alert dispatch via Email completed successfully"
                    : "Attempt to send an email alert failed!");

                LogMessage(alert.SendSMSAlert("Atert: Energize/Denergize Relay request to NetDuino Failed", msg)
                    ? "Alert dispatch via SMS completed successfully"
                    : "Attempt to send an SMS alert failed!");
            }
            
            return stateChangeInterval;
        }

        /// <summary>
        /// Sends commands to set the current state of the NetDuino Relay R2 (AC Relay) based on the 
        /// configured OnTime and OffTime values
        /// </summary>
        /// <returns> returns a TimeSpan that tells us when we need to call this method again</returns>
        private static async Task<TimeSpan> SetNetDuinoACRelay()
        {
            string result;
            var alert = new AlertSender();

            // First calulate the DC Bus On Time value using Today's Sunset time &
            // given on Time offset value:
            DateTime sunriseToday, sunsetToday;
            SunTimes.GetSunTimes(out sunriseToday, out sunsetToday);

            var onTime = sunsetToday - new TimeSpan(0, 0, _ACBusOnTimeOffset, 0);
            var offTime = _ACBusOffTime;

            if (onTime < offTime)
            {
                LogMessage("Invalid Configuration!. Please check the On/Off Time values");
                return TimeSpan.MinValue;
            }

            // time to wait for next state change trigger
            TimeSpan stateChangeInterval;

            if (DateTime.Now.TimeOfDay < onTime.TimeOfDay)
            {
                // we are in daytime
                // energize the relay 1 to turn lights off and set the interval for next state change
                result = await NetDuinoPlus.EnergizeRelay2();
                stateChangeInterval = onTime.TimeOfDay - DateTime.Now.TimeOfDay;
            }
            else if (DateTime.Now.TimeOfDay >= onTime.TimeOfDay && DateTime.Now.TimeOfDay <= offTime.TimeOfDay)
            {
                // we are in the onTime..
                // de-energize relay1 to turn the lights on and set then to turn off at offTime
                result = await NetDuinoPlus.DenergizeRelay2();
                stateChangeInterval = offTime.TimeOfDay - DateTime.Now.TimeOfDay;
            }
            else
            {
                // Current time is between OffTime and midnight
                // energize the relays to turn the light off and set the interval to onTime + 1 Day
                result = await NetDuinoPlus.EnergizeRelay2();
                stateChangeInterval = (new TimeSpan(1, 0, 0, 0) + onTime.TimeOfDay) - DateTime.Now.TimeOfDay;
            }

            if (result == "Success")
            {
                if (_changeStateR2Timer == null)
                {
                    //This is the first time this method is executed
                    // set up the timer to trigger next time the Relay state change is needed: 
                    _changeStateR2Timer = new Timer(OnChangeStateR2Timer, null, stateChangeInterval, Timeout.InfiniteTimeSpan);
                }
            }
            else
            {
                // here we deal with the failure...
                // set the TimeSpan to min value to return an indication of failure and log appropriate messages and alerts
                stateChangeInterval = TimeSpan.MinValue;

                var @address = NetDuinoPlus.DeviceIPAddress.Substring(7, 13);

                var msg =
                    "Netduino has failed to respond to Energize/Denergize AC relay request(s) dispatched to address: " + @address + "in a timely fashion" +
                    $"{Environment.NewLine}Event Date & Time: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()} {Environment.NewLine}" +
                    $"{Environment.NewLine}Please check Netduino and make sure that it is still online!";

                LogMessage(alert.SendEmailAlert("Atert: Energize/Denergize AC Relay request to NetDuino Failed", bodyText: msg)
                    ? "Alert dispatch via Email completed successfully"
                    : "Attempt to send an email alert failed!");

                LogMessage(alert.SendSMSAlert("Atert: Energize/Denergize Relay request to NetDuino Failed", msg)
                    ? "Alert dispatch via SMS completed successfully"
                    : "Attempt to send an SMS alert failed!");
            }

            return stateChangeInterval;
        }

        #region Timer event Handler methods

        private async void OnPingTimer(object state)
        {
            // send a ping asynchronously and reset the timer

            await _netDuino.ExecutePingAsync(LogMessage);

            var pingInterval = new TimeSpan(0, 0, 1, 0); // 1 minute
            _pingtimer.Change(pingInterval, Timeout.InfiniteTimeSpan);
        }

        private static async void OnChangeStateR1Timer(object state)
        {
           var stateChangeInterval =  await SetNetDuinoDCRelay();

            if (stateChangeInterval > TimeSpan.MinValue)
            {
                _changeStateR1Timer.Change(stateChangeInterval, Timeout.InfiniteTimeSpan);
            }
        }

        private static async void OnChangeStateR2Timer(object state)
        {
            var stateChangeInterval = await SetNetDuinoACRelay();

            if (stateChangeInterval > TimeSpan.MinValue)
            {
                _changeStateR2Timer.Change(stateChangeInterval, Timeout.InfiniteTimeSpan);
            }
        }

        #endregion


        private static void LogMessage(string msg )
        {
            if (_consoleMode)
            {
                Console.WriteLine(msg);
            }
            else
            {
                _logger.Info(msg);      
            }
        }


        public void Stop()
        {
            if(_consoleMode)
            {
                Console.WriteLine($"Charon Service Stop requested at {DateTime.Now}");
                _logger.Info("Exiting Charon Service Application...");

                var n = 3;
                while (n > 0)
                {
                    Console.Write($"\rStopping application in {n} seconds");
                    Thread.Sleep(1000);
                    n--;
                }
            }
            else
            {
                _logger.Info("Stopping Charon Service Application");
            } 
        }
    }
}
