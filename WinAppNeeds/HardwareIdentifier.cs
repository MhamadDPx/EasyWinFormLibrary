using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Simplified Hardware identification class for software activation purposes
    /// </summary>
    public static class HardwareIdentifier
    {
        #region Public Methods

        /// <summary>
        /// Gets a unique hardware identifier for the current machine
        /// </summary>
        /// <returns>16-character unique hardware ID</returns>
        public static string GetUniqueId()
        {
            try
            {
                var components = new List<string>();

                // Collect hardware components
                components.Add(GetCpuId());
                components.Add(GetMotherboardSerial());
                components.Add(GetBiosSerial());
                components.Add(GetFirstHardDriveSerial());

                // Remove empty components
                components = components.Where(c => !string.IsNullOrEmpty(c)).ToList();

                if (components.Count == 0)
                {
                    // Fallback to MAC address if no other components available
                    components.Add(GetMacAddress());
                }

                if (components.Count == 0)
                {
                    throw new Exception("Unable to generate hardware identifier - no hardware components found");
                }

                // Combine all components
                string combined = string.Join("-", components);

                // Generate hash and return first 16 characters
                return ComputeSHA256Hash(combined).Substring(0, 16);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate hardware identifier: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a longer unique hardware identifier (32 characters)
        /// </summary>
        /// <returns>32-character unique hardware ID</returns>
        public static string GetLongUniqueId()
        {
            try
            {
                var components = new List<string>();

                // Collect all available hardware components
                components.Add(GetCpuId());
                components.Add(GetMotherboardSerial());
                components.Add(GetBiosSerial());
                components.Add(GetFirstHardDriveSerial());
                components.Add(GetMacAddress());
                components.Add(GetSystemUuid());

                // Remove empty components
                components = components.Where(c => !string.IsNullOrEmpty(c)).ToList();

                if (components.Count == 0)
                {
                    throw new Exception("Unable to generate hardware identifier - no hardware components found");
                }

                // Combine all components
                string combined = string.Join("-", components);

                // Generate hash and return first 32 characters
                return ComputeSHA256Hash(combined).Substring(0, 32);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate long hardware identifier: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets detailed hardware information for debugging/support
        /// </summary>
        /// <returns>Dictionary containing hardware component details</returns>
        public static Dictionary<string, string> GetHardwareDetails()
        {
            var details = new Dictionary<string, string>();

            try { details["CPU_ID"] = GetCpuId(); } catch { details["CPU_ID"] = "N/A"; }
            try { details["Motherboard_Serial"] = GetMotherboardSerial(); } catch { details["Motherboard_Serial"] = "N/A"; }
            try { details["BIOS_Serial"] = GetBiosSerial(); } catch { details["BIOS_Serial"] = "N/A"; }
            try { details["HDD_Serial"] = GetFirstHardDriveSerial(); } catch { details["HDD_Serial"] = "N/A"; }
            try { details["MAC_Address"] = GetMacAddress(); } catch { details["MAC_Address"] = "N/A"; }
            try { details["System_UUID"] = GetSystemUuid(); } catch { details["System_UUID"] = "N/A"; }
            try { details["Unique_ID"] = GetUniqueId(); } catch { details["Unique_ID"] = "N/A"; }

            return details;
        }

        /// <summary>
        /// Validates if the current hardware matches a previously generated ID
        /// </summary>
        /// <param name="previousId">Previously generated hardware ID</param>
        /// <returns>True if hardware matches</returns>
        public static bool ValidateHardwareId(string previousId)
        {
            try
            {
                if (string.IsNullOrEmpty(previousId))
                    return false;

                string currentId = GetUniqueId();
                return string.Equals(currentId, previousId, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Private Hardware Collection Methods

        /// <summary>
        /// Gets the CPU processor ID
        /// </summary>
        private static string GetCpuId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var cpuId = obj["ProcessorId"]?.ToString();
                        if (!string.IsNullOrEmpty(cpuId))
                            return CleanString(cpuId);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting CPU ID: {ex.Message}");
            }
            return "";
        }

        /// <summary>
        /// Gets the motherboard serial number
        /// </summary>
        private static string GetMotherboardSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var serial = obj["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(serial) &&
                            !serial.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                            !serial.Equals("To be filled by O.E.M.", StringComparison.OrdinalIgnoreCase))
                            return CleanString(serial);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting motherboard serial: {ex.Message}");
            }
            return "";
        }

        /// <summary>
        /// Gets the BIOS serial number
        /// </summary>
        private static string GetBiosSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var serial = obj["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(serial) &&
                            !serial.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                            !serial.Equals("To be filled by O.E.M.", StringComparison.OrdinalIgnoreCase))
                            return CleanString(serial);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting BIOS serial: {ex.Message}");
            }
            return "";
        }

        /// <summary>
        /// Gets the first physical hard drive serial number
        /// </summary>
        private static string GetFirstHardDriveSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var serial = obj["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(serial))
                            return CleanString(serial);
                    }
                }

                // Alternative method using Win32_DiskDrive
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE MediaType='Fixed hard disk media'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var serial = obj["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(serial))
                            return CleanString(serial);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting HDD serial: {ex.Message}");
            }
            return "";
        }

        /// <summary>
        /// Gets the MAC address of the first network adapter
        /// </summary>
        private static string GetMacAddress()
        {
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                               n.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                    .OrderBy(n => n.Name);

                foreach (var networkInterface in networkInterfaces)
                {
                    var macAddress = networkInterface.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(macAddress) && macAddress != "000000000000")
                        return macAddress;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting MAC address: {ex.Message}");
            }
            return "";
        }

        /// <summary>
        /// Gets the system UUID
        /// </summary>
        private static string GetSystemUuid()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var uuid = obj["UUID"]?.ToString();
                        if (!string.IsNullOrEmpty(uuid) &&
                            !uuid.Equals("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", StringComparison.OrdinalIgnoreCase))
                            return CleanString(uuid);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting system UUID: {ex.Message}");
            }
            return "";
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Cleans a string by removing spaces and converting to uppercase
        /// </summary>
        private static string CleanString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace(" ", "").Replace("-", "").ToUpperInvariant();
        }

        /// <summary>
        /// Computes SHA256 hash of input string
        /// </summary>
        private static string ComputeSHA256Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
            }
        }

        #endregion
    }
}