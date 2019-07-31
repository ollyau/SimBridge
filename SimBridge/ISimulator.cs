using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{

    //-----------------------------------------------------------------------------

    enum LightType
    {
        Beacon, // TOGGLE_BEACON_LIGHTS + simvar ( LIGHT BEACON )
        Strobe, // STROBES_SET
        Navigation, // TOGGLE_NAV_LIGHTS + simvar ( LIGHT NAV )
        Landing, // LANDING_LIGHTS_SET
        Taxi, // TOGGLE_TAXI_LIGHTS + simvar ( LIGHT TAXI )
        Panel // PANEL_LIGHTS_SET
    }

    enum RadioType
    {
        //COM_ACTIVE, // check simvar ( COM TRANSMIT:index, COM STATUS:index )
        COM_1, // COM_STBY_RADIO_SET + COM_STBY_RADIO_SWAP
        COM_2, // COM2_STBY_RADIO_SET + COM2_RADIO_SWAP
        NAV_1, // NAV1_STBY_SET + NAV1_RADIO_SWAP
        NAV_2 // NAV2_STBY_SET + NAV2_RADIO_SWAP
    }

    //-----------------------------------------------------------------------------

    interface ISimulator
    {
        void SetPause(bool paused); // PAUSE_SET
        void SetSound(bool on); // SOUND_SET
        void CaptureScreenshot(); // CAPTURE_SCREENSHOT
        void ResetFlight(); // SITUATION_RESET
        void QuitWithoutMessage(); // ABORT (quits without message)
        
        void ReloadAircraft(); // RELOAD_USER_AIRCRAFT
        void ReloadScenery(); // REFRESH_SCENERY

        void NextViewCategory(); // VIEW_MODE
        void NextSubView(); // NEXT_SUB_VIEW
        void PreviousViewCategory(); // VIEW_MODE_REV
        void PreviousSubView(); // PREV_SUB_VIEW
        void SetVirtualCockpit(); // VIEW_CAMERA_SELECT_1 (assumes it's the usual selection) || VIEW_VIRTUAL_COCKPIT_FORWARD maybe?
        // void ResetEyepoint(); // EYEPOINT_RESET

        void SetTransponder(int code); // XPNDR_SET
        void SetAltimeter(double inHg); // KOHLSMAN_SET
        void SetRadioStandby(RadioType radio, double MHz);
        void SwapRadio(RadioType radio);

        void SetLights(LightType light, bool on);
        void SetPitotHeat(bool on); // PITOT_HEAT_SET

        void FlapsUp(); // FLAPS_UP
        void RaiseFlaps(); // FLAPS_DECR
        void LowerFlaps(); // FLAPS_INCR
        void FlapsFull(); // FLAPS_DOWN
        void SetGear(bool down); // GEAR_SET
        // void SetExit(bool open); // TOGGLE_AIRCRAFT_EXIT + simvar ( EXIT OPEN:index )
        // void SetFlaps(int degrees); // FLAPS_SET or AXIS_FLAPS_SET + parse aircraft.cfg

        // void SetAutopilot(bool on); // AUTOPILOT_OFF or AUTOPILOT_ON
        // void SetHeadingHold(bool on); // AP_HDG_HOLD_ON / AP_HDG_HOLD_OFF || AP_PANEL_HEADING_SET
        // void SetHeadingBug(float degrees); // HEADING_BUG_SET
        // void SetAltitudeHold(bool on); // AP_ALT_HOLD_ON / AP_ALT_HOLD_OFF || AP_PANEL_ALTITUDE_SET
        // void SetRefAltitude(float feet); // AP_ALT_VAR_SET_ENGLISH

        // todo:
        // * throttle RPM control via PID controller?
        // * mixture lean to peak EGT or whatever via PID controller?
        // * failures?
        // * text display?
    }

    //-----------------------------------------------------------------------------

}
