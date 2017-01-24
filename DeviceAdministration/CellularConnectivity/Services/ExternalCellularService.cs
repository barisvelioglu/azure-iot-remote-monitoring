﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManagement.Infrustructure.Connectivity.Clients;
using DeviceManagement.Infrustructure.Connectivity.Models.Constants;
using DeviceManagement.Infrustructure.Connectivity.Models.Other;
using DeviceManagement.Infrustructure.Connectivity.Models.Security;
using DeviceManagement.Infrustructure.Connectivity.Models.TerminalDevice;

namespace DeviceManagement.Infrustructure.Connectivity.Services
{
    public class ExternalCellularService : IExternalCellularService
    {
        private readonly ICredentialProvider _credentialProvider;

        public ExternalCellularService(ICredentialProvider credentialProvider)
        {
            _credentialProvider = credentialProvider;
        }

        public List<Iccid> GetTerminals()
        {
            List<Iccid> terminals;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    terminals = new JasperCellularClient(_credentialProvider).GetTerminals();
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    terminals = new EricssonCellularClient(_credentialProvider).GetTerminals();
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }
            return terminals;
        }

        public Terminal GetSingleTerminalDetails(Iccid iccid)
        {
            Terminal terminal;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    terminal = new JasperCellularClient(_credentialProvider).GetSingleTerminalDetails(iccid);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    terminal = new EricssonCellularClient(_credentialProvider).GetSingleTerminalDetails(iccid);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }

            return terminal;
        }

        public List<SessionInfo> GetSingleSessionInfo(Iccid iccid)
        {
            List<SessionInfo> sessionInfo = null;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    sessionInfo = new JasperCellularClient(_credentialProvider).GetSingleSessionInfo(iccid);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    sessionInfo = new EricssonCellularClient(_credentialProvider).GetSingleSessionInfo(iccid);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }

            return sessionInfo;
        }

        public List<SimState> GetAllAvailableSimStates(string iccid)
        {
            List<SimState> availableStates;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    availableStates = jasperClient.GetAvailableSimStates(iccid);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    availableStates = ericssonClient.GetAllAvailableSimStates();
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }
            return availableStates;
        }

        public List<SimState> GetValidTargetSimStates(string iccid, string currentStateId)
        {
            List<SimState> availableStates;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    availableStates = jasperClient.GetValidTargetSimStates(iccid, currentStateId);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    availableStates = ericssonClient.GetValidTargetSimStates(currentStateId);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }
            return availableStates;
        }

        public List<SubscriptionPackage> GetAvailableSubscriptionPackages(string iccid, string currentSubscription)
        {
            List<SubscriptionPackage> availableSubscriptionPackages;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    availableSubscriptionPackages = jasperClient.GetAvailableSubscriptionPackages(iccid, currentSubscription);
                    break;

                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    availableSubscriptionPackages = ericssonClient.GetAvailableSubscriptionPackages(iccid, currentSubscription);
                    break;
                default:

                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }

            return availableSubscriptionPackages;
        }

        public bool UpdateSimState(string iccid, string updatedState)
        {
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;
            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    var jasperResult = jasperClient.UpdateSimState(iccid, updatedState);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    var status = SubscriptionStatusFactory.CreateEricssonSubscriptionStatusRequestEnum(updatedState);
                    var ericssonResult = ericssonClient.UpdateSimState(iccid, status);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }
            return true;
        }

        /// <summary>
        /// Update the subscription package in the selected api registration provider
        /// </summary>
        /// <param name="iccid">The iccid string for the terminal</param>
        /// <param name="updatedPackage">The string value for the updated package name</param>
        /// <returns>A boolean representing success or failure</returns>
        public bool UpdateSubscriptionPackage(string iccid, string updatedPackage)
        {
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;
            // TODO SR: Figure out if we need to do something with responses
            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    var jasperResponse = jasperClient.UpdateSubscriptionPackage(iccid, updatedPackage);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    var ericssonResponse = ericssonClient.UpdateSubscriptionPackage(iccid, updatedPackage);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }
            return true;
        }

        public bool ReconnectTerminal(string iccid)
        {
            bool result;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    result = jasperClient.ReconnectTerminal(iccid);
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    ericssonClient.ReconnectTerminal(iccid);
                    result = true;
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }
            return result;
        }

        public async Task<bool> SendSms(string iccid, string msisdn, string smsText)
        {
            bool result;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    var jasperClient = new JasperCellularClient(_credentialProvider);
                    var response = jasperClient.SendSms(iccid, smsText);
                    //TODO SR: Should we do something with the response?
                    result = true;
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    var ericssonClient = new EricssonCellularClient(_credentialProvider);
                    result = await ericssonClient.SendSMS(msisdn, smsText);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider}' provider");
            }
            return result;
        }

        /// <summary>
        /// The API does not have a way to validate credentials so this method calls
        /// GetTerminals() and checks the response for validation errors.
        /// </summary>
        /// <returns>True if valid. False if not valid</returns>
        public bool ValidateCredentials()
        {
            bool isValid;
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    isValid = new JasperCellularClient(_credentialProvider).ValidateCredentials();
                    break;
                case ApiRegistrationProviderTypes.Ericsson:
                    isValid = new EricssonCellularClient(_credentialProvider).ValidateCredentials();
                    break;
                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }

            return isValid;
        }

        /// <summary>
        /// Get current locale name and available locale names
        /// </summary>
        /// <param name="iccid">ICCID</param>
        /// <param name="availableLocaleNames">Available locale names</param>
        /// <returns>Current locale name</returns>
        public string GetLocale(string iccid, out IEnumerable<string> availableLocaleNames)
        {
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    throw new NotSupportedException();

                case ApiRegistrationProviderTypes.Ericsson:
                    var client = new EricssonCellularClient(_credentialProvider);
                    return client.GetLocale(iccid, out availableLocaleNames);

                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }
        }

        /// <summary>
        /// Set locale
        /// </summary>
        /// <param name="iccid">ICCID</param>
        /// <param name="localeName">Desired locale name</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public string SetLocale(string iccid, string localeName)
        {
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    throw new NotSupportedException();

                case ApiRegistrationProviderTypes.Ericsson:
                    var client = new EricssonCellularClient(_credentialProvider);
                    return client.SetLocale(iccid, localeName);

                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }
        }

        public string GetLastSetLocaleServiceRequestState(string serviceRequestId)
        {
            var registrationProvider = _credentialProvider.Provide().ApiRegistrationProvider;

            switch (registrationProvider)
            {
                case ApiRegistrationProviderTypes.Jasper:
                    throw new NotSupportedException();

                case ApiRegistrationProviderTypes.Ericsson:
                    var client = new EricssonCellularClient(_credentialProvider);
                    return client.GetLastSetLocaleServiceRequestState(serviceRequestId);

                default:
                    throw new IndexOutOfRangeException($"Could not find a service for '{registrationProvider.ToString()}' provider");
            }
        }

        private List<SimState> getAvailableSimStates()
        {
            return new List<SimState>()
            {
                new SimState()
                {
                    Name = "Active"
                },
                new SimState()
                {
                    Name = "Disconnected"
                }
            };
        }

        /// <summary>
        /// Gets the available subscription packages from the appropriate api provider
        /// </summary>
        /// <returns>SubscriptionPackage Model</returns>
        private List<SubscriptionPackage> getAvailableSubscriptionPackages()
        {
            return new List<SubscriptionPackage>()
            {
                new SubscriptionPackage()
                {
                    Name = "Basic"
                },
                new SubscriptionPackage()
                {
                    Name = "Expensive"
                }
            };
        }
    }
}