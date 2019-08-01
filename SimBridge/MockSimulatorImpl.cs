using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    class MockSimulatorImpl : ISimulator
    {
        public void SetPause(bool paused)
        {
            Log.Instance.Debug($"SetPause (paused = {paused})");
        }

        public void SetSound(bool on)
        {
            Log.Instance.Debug($"SetSound (on = {on})");
        }

        public void CaptureScreenshot()
        {
            Log.Instance.Debug("CaptureScreenshot");
        }

        public void ResetFlight()
        {
            Log.Instance.Debug("ResetFlight");
        }

        public void QuitWithoutMessage()
        {
            Log.Instance.Debug("QuitWithoutMessage");
        }

        public void ReloadAircraft()
        {
            Log.Instance.Debug("ReloadAircraft");
        }

        public void ReloadScenery()
        {
            Log.Instance.Debug("ReloadScenery");
        }

        public void NextViewCategory()
        {
            Log.Instance.Debug("NextViewCategory");
        }

        public void NextSubView()
        {
            Log.Instance.Debug("NextSubView");
        }

        public void PreviousViewCategory()
        {
            Log.Instance.Debug("PreviousViewCategory");
        }

        public void PreviousSubView()
        {
            Log.Instance.Debug("PreviousSubView");
        }

        public void SetVirtualCockpit()
        {
            Log.Instance.Debug("SetVirtualCockpit");
        }

        public void SetTransponder(int code)
        {
            Log.Instance.Debug($"SetTransponder (code = {code})");
        }

        public void SetAltimeter(double inHg)
        {
            // https://www.nist.gov/physical-measurement-laboratory/nist-guide-si-appendix-b8
            var millibars = inHg * 33.86389;
            Log.Instance.Debug($"SetAltimeter (inHg = {inHg}, millibars = {millibars})");
        }

        public void SetRadioStandby(RadioType radio, double MHz)
        {
            Log.Instance.Debug($"SetRadioStandby (radio = {radio}, MHz = {MHz})");
        }

        public void SwapRadio(RadioType radio)
        {
            Log.Instance.Debug($"SwapRadio (radio = {radio})");
        }

        public void SetLights(LightType light, bool on)
        {
            Log.Instance.Debug($"SetLights (light = {light}, on = {on})");
        }

        public void SetPitotHeat(bool on)
        {
            Log.Instance.Debug($"SetPitotHeat (on = {on})");
        }

        public void FlapsUp()
        {
            Log.Instance.Debug($"FlapsUp");
        }

        public void RaiseFlaps()
        {
            Log.Instance.Debug($"RaiseFlaps");
        }

        public void LowerFlaps()
        {
            Log.Instance.Debug($"LowerFlaps");
        }

        public void FlapsFull()
        {
            Log.Instance.Debug($"FlapsFull");
        }

        public void SetGear(bool down)
        {
            Log.Instance.Debug($"SetGear (down = {down})");
        }
    }
}
