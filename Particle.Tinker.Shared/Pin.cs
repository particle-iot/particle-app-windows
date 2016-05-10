using Particle.SDK;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Particle.Tinker
{
    #region Enums

    public enum PinAction
    {
        DigitalRead,
        DigitalWrite,
        AnalogRead,
        AnalogWrite,
        AnalogWriteDac,
        None
    }

    public enum PinType
    {
        A,
        B,
        C,
        D
    }

    #endregion

    public class Pin : INotifyPropertyChanged
    {
        #region Constants

        public const int AnalogReadMax = 4095;
        public const int AnalogWriteMax = 255;
        public const int AnalogWriteMaxAlt = AnalogReadMax;

        public static PinAction[] AllFunctions = { PinAction.AnalogRead, PinAction.AnalogWrite, PinAction.DigitalRead, PinAction.DigitalWrite };
        public static PinAction[] AllFunctionsDAC = { PinAction.AnalogRead, PinAction.AnalogWriteDac, PinAction.DigitalRead, PinAction.DigitalWrite };
        public static PinAction[] NoAnalogWrite = { PinAction.AnalogRead, PinAction.DigitalRead, PinAction.DigitalWrite };
        public static PinAction[] NoAnalogRead = { PinAction.AnalogWrite, PinAction.DigitalRead, PinAction.DigitalWrite };
        public static PinAction[] DigitalOnly = { PinAction.DigitalRead, PinAction.DigitalWrite };

        #endregion

        #region Private Members

        private string tinkerId;
        private PinType pinType;
        private string name;
        private PinAction[] functions;
        private string caption;
        private int maxAnalogWriteValue;
        private PinAction configuredAction;
        private int value;
        private string valueText;
        private double valuePercent;
        private bool valueKnown;
        private bool syncing;
        private bool requestError;
        private ParticleDevice particleDevice;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public string TinkerId
        {
            get { return tinkerId; }
            protected set
            {
                tinkerId = value;
                OnPropertyChanged("TinkerId");
            }
        }

        public PinType PinType
        {
            get { return pinType; }
            protected set
            {
                pinType = value;
                OnPropertyChanged("PinType");
            }
        }

        public string Name
        {
            get { return name; }
            protected set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public PinAction[] Functions
        {
            get { return functions; }
            protected set
            {
                functions = value;
                OnPropertyChanged("Functions");
            }
        }

        public string Caption
        {
            get { return caption; }
            protected set
            {
                caption = value;
                OnPropertyChanged("Caption");
            }
        }

        public int MaxAnalogWriteValue
        {
            get { return maxAnalogWriteValue; }
            protected set
            {
                maxAnalogWriteValue = value;
                OnPropertyChanged("MaxAnalogWriteValue");
            }
        }

        public PinAction ConfiguredAction
        {
            get { return configuredAction; }
            set
            {
                ResetPin(value);
            }
        }

        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;

                switch (ConfiguredAction)
                {
                    case PinAction.AnalogRead:
                    case PinAction.AnalogWrite:
                    case PinAction.AnalogWriteDac:
                        valueText = value.ToString();
                        break;
                    case PinAction.DigitalRead:
                    case PinAction.DigitalWrite:
                        if (Value == 1)
                            valueText = "HIGH";
                        else
                            valueText = "LOW";
                        break;
                    case PinAction.None:
                    default:
                        valueText = "";
                        break;
                }

                switch (configuredAction)
                {
                    case PinAction.AnalogRead:
                        valuePercent = value / (double)AnalogReadMax * 100;
                        break;
                    case PinAction.AnalogWrite:
                    case PinAction.AnalogWriteDac:
                        valuePercent = value / (double)MaxAnalogWriteValue * 100;
                        break;
                    case PinAction.DigitalRead:
                    case PinAction.DigitalWrite:
                    case PinAction.None:
                    default:
                        valuePercent = 0;
                        break;
                }

                valueKnown = true;

                OnPropertyChanged("Value");
                OnPropertyChanged("ValueText");
                OnPropertyChanged("ValuePercent");
                OnPropertyChanged("ValueKnown");
            }
        }

        public string ValueText
        {
            get { return valueText; }
        }

        public double ValuePercent
        {
            get { return valuePercent; }
        }

        public bool ValueKnown
        {
            get { return valueKnown; }
        }

        public bool Syncing
        {
            get { return syncing; }
            protected set
            {
                syncing = value;
                OnPropertyChanged("Syncing");
            }
        }

        public bool RequestError
        {
            get { return requestError; }
            protected set
            {
                requestError = value;
                OnPropertyChanged("RequestError");
            }
        }

        public ParticleDevice ParticleDevice
        {
            get { return particleDevice; }
            protected set
            {
                particleDevice = value;
                OnPropertyChanged("ParticleDevice");
            }
        }

        #endregion

        #region Constructors

        public Pin(string tinkerId, PinType pinType, string name, PinAction[] functions, string caption = null, int maxAnalogWriteValue = AnalogWriteMax)
        {
            this.tinkerId = tinkerId;
            this.pinType = pinType;
            this.name = name;
            this.functions = functions;
            if (string.IsNullOrWhiteSpace(caption))
                this.caption = name;
            else
                this.caption = caption;
            this.maxAnalogWriteValue = maxAnalogWriteValue;
            syncing = false;
            ResetPin();
        }

        #endregion

        #region Pubic Methods

        public async void AnalogReadAsync()
        {
            await RunFunctionAsync("analogread", $"{name}");
        }

        public async void AnalogWriteAsync(int new_value)
        {
            await RunFunctionAsync("analogwrite", $"{name} {new_value}", new_value);
        }

        public async Task DigitalReadAsync()
        {
            await RunFunctionAsync("digitalread", $"{name}");
        }

        public async void DigitalWriteAsync()
        {
            string arg = valueText == "HIGH" ? "LOW" : "HIGH";
            int new_value = valueText == "HIGH" ? 0 : 1;
            await RunFunctionAsync("digitalwrite", $"{name} {arg}", new_value);
        }

        public static List<Pin> GetDevicePins(ParticleDevice particleDevice)
        {
            List<Pin> allPins = new List<Pin>();

            switch (particleDevice.PlatformId)
            {
                case ParticleDeviceType.Core:
                    allPins.Add(new Pin("TinkerA7", PinType.A, "A7", AllFunctions));
                    allPins.Add(new Pin("TinkerA6", PinType.A, "A6", AllFunctions));
                    allPins.Add(new Pin("TinkerA5", PinType.A, "A5", AllFunctions));
                    allPins.Add(new Pin("TinkerA4", PinType.A, "A4", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerA3", PinType.A, "A3", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerA2", PinType.A, "A2", AllFunctions));
                    allPins.Add(new Pin("TinkerA1", PinType.A, "A1", AllFunctions));
                    allPins.Add(new Pin("TinkerA0", PinType.A, "A0", AllFunctions));

                    allPins.Add(new Pin("TinkerD7", PinType.D, "D7", DigitalOnly));
                    allPins.Add(new Pin("TinkerD6", PinType.D, "D6", DigitalOnly));
                    allPins.Add(new Pin("TinkerD5", PinType.D, "D5", DigitalOnly));
                    allPins.Add(new Pin("TinkerD4", PinType.D, "D4", DigitalOnly));
                    allPins.Add(new Pin("TinkerD3", PinType.D, "D3", DigitalOnly));
                    allPins.Add(new Pin("TinkerD2", PinType.D, "D2", DigitalOnly));
                    allPins.Add(new Pin("TinkerD1", PinType.D, "D1", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD0", PinType.D, "D0", NoAnalogRead));

                    break;

                case ParticleDeviceType.Photon:
                case ParticleDeviceType.P1:
                    allPins.Add(new Pin("TinkerA7", PinType.A, "A7", AllFunctions, "WKP", AnalogWriteMax));
                    allPins.Add(new Pin("TinkerA6", PinType.A, "A6", AllFunctionsDAC, "DAC", AnalogWriteMaxAlt));
                    allPins.Add(new Pin("TinkerA5", PinType.A, "A5", AllFunctions));
                    allPins.Add(new Pin("TinkerA4", PinType.A, "A4", AllFunctions));
                    allPins.Add(new Pin("TinkerA3", PinType.A, "A3", AllFunctionsDAC, "A3", AnalogWriteMaxAlt));
                    allPins.Add(new Pin("TinkerA2", PinType.A, "A2", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerA1", PinType.A, "A1", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerA0", PinType.A, "A0", NoAnalogWrite));

                    allPins.Add(new Pin("TinkerD7", PinType.D, "D7", DigitalOnly));
                    allPins.Add(new Pin("TinkerD6", PinType.D, "D6", DigitalOnly));
                    allPins.Add(new Pin("TinkerD5", PinType.D, "D5", DigitalOnly));
                    allPins.Add(new Pin("TinkerD4", PinType.D, "D4", DigitalOnly));
                    allPins.Add(new Pin("TinkerD3", PinType.D, "D3", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD2", PinType.D, "D2", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD1", PinType.D, "D1", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD0", PinType.D, "D0", NoAnalogRead));

                    break;

                case ParticleDeviceType.Electron:
                    allPins.Add(new Pin("TinkerA7", PinType.A, "A7", AllFunctions, "WKP", AnalogWriteMax));
                    allPins.Add(new Pin("TinkerA6", PinType.A, "A6", AllFunctionsDAC, "DAC", AnalogWriteMaxAlt));
                    allPins.Add(new Pin("TinkerA5", PinType.A, "A5", AllFunctions));
                    allPins.Add(new Pin("TinkerA4", PinType.A, "A4", AllFunctions));
                    allPins.Add(new Pin("TinkerA3", PinType.A, "A3", AllFunctionsDAC, "A3", AnalogWriteMaxAlt));
                    allPins.Add(new Pin("TinkerA2", PinType.A, "A2", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerA1", PinType.A, "A1", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerA0", PinType.A, "A0", NoAnalogWrite));

                    allPins.Add(new Pin("TinkerB5", PinType.B, "B5", NoAnalogWrite));
                    allPins.Add(new Pin("TinkerB4", PinType.B, "B4", AllFunctions));
                    allPins.Add(new Pin("TinkerB3", PinType.B, "B3", AllFunctions));
                    allPins.Add(new Pin("TinkerB2", PinType.B, "B2", AllFunctions));
                    allPins.Add(new Pin("TinkerB1", PinType.B, "B1", NoAnalogRead));
                    allPins.Add(new Pin("TinkerB0", PinType.B, "B0", DigitalOnly));

                    allPins.Add(new Pin("TinkerD7", PinType.D, "D7", DigitalOnly));
                    allPins.Add(new Pin("TinkerD6", PinType.D, "D6", DigitalOnly));
                    allPins.Add(new Pin("TinkerD5", PinType.D, "D5", DigitalOnly));
                    allPins.Add(new Pin("TinkerD4", PinType.D, "D4", DigitalOnly));
                    allPins.Add(new Pin("TinkerD3", PinType.D, "D3", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD2", PinType.D, "D2", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD1", PinType.D, "D1", NoAnalogRead));
                    allPins.Add(new Pin("TinkerD0", PinType.D, "D0", NoAnalogRead));

                    allPins.Add(new Pin("TinkerC5", PinType.C, "C5", NoAnalogRead));
                    allPins.Add(new Pin("TinkerC4", PinType.C, "C4", NoAnalogRead));
                    allPins.Add(new Pin("TinkerC3", PinType.C, "C3", DigitalOnly));
                    allPins.Add(new Pin("TinkerC2", PinType.C, "C2", DigitalOnly));
                    allPins.Add(new Pin("TinkerC1", PinType.C, "C1", DigitalOnly));
                    allPins.Add(new Pin("TinkerC0", PinType.C, "C0", DigitalOnly));
                    break;
            }

            foreach (var pin in allPins)
                pin.ParticleDevice = particleDevice;

            return allPins;
        }

        public void ResetPin(PinAction pinAction = PinAction.None)
        {
            configuredAction = pinAction;
            value = 0;
            valueText = "";
            valuePercent = 0;
            valueKnown = false;

            OnPropertyChanged("ConfiguredAction");
            OnPropertyChanged("Value");
            OnPropertyChanged("ValueText");
            OnPropertyChanged("ValuePercent");
            OnPropertyChanged("ValueKnown");
        }

        #endregion

        #region Private Methods

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task RunFunctionAsync(string function, string arg, int? new_value = null)
        {
            RequestError = false;
            Syncing = true;

            try
            {
                var getValue = await particleDevice.RunFunctionAsync(function, arg);
                if (getValue == null)
                {
                    RequestError = true;
                }
                else {
                    switch (function)
                    {
                        case "digitalread":
                            Value = getValue.ReturnValue;
                            break;
                        case "digitalwrite":
                            if (getValue.ReturnValue == 1)
                                Value = new_value.Value;
                            break;
                        case "analogread":
                            Value = getValue.ReturnValue;
                            break;
                        case "analogwrite":
                            if (getValue.ReturnValue == 1)
                                Value = new_value.Value;
                            break;
                    }
                }
            }
            catch
            {
                RequestError = true;
            }

            Syncing = false;
        }

        #endregion
    }
}
