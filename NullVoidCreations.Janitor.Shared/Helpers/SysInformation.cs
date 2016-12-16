using System.Collections.Generic;
using System.Management;

namespace NullVoidCreations.Janitor.Shared.Helpers
{
    public class SysInformation
    {
        public class ManagementClassNames
        {
            public const string Controller1394 = "Win32_1394Controller";
            public const string ControllerDevice1394 = "Win32_1394ControllerDevice";
            public const string AccountSID = "Win32_AccountSID";
            public const string ActionCheck = "Win32_ActionCheck";
            public const string ActiveRoute = "Win32_ActiveRoute";
            public const string AllocatedResource = "Win32_AllocatedResource";
            public const string ApplicationCommandLine = "Win32_ApplicationCommandLine";
            public const string ApplicationService = "Win32_ApplicationService";
            public const string AssociatedBattery = "Win32_AssociatedBattery";
            public const string AssociatedProcessorMemory = "Win32_AssociatedProcessorMemory";
            public const string AutochkSetting = "Win32_AutochkSetting";
            public const string BaseBoard = "Win32_BaseBoard";
            public const string Battery = "Win32_Battery";
            public const string Binary = "Win32_Binary";
            public const string BindImageAction = "Win32_BindImageAction";
            public const string BIOS = "Win32_BIOS";
            public const string BootConfiguration = "Win32_BootConfiguration";
            public const string Bus_Win32_CacheMemory = "Win32_Bus Win32_CacheMemory";
            public const string CDROMDrive = "Win32_CDROMDrive";
            public const string CheckCheck = "Win32_CheckCheck";
            public const string CIMLogicalDeviceCIMDataFile = "Win32_CIMLogicalDeviceCIMDataFile";
            public const string ClassicCOMApplicationClasses = "Win32_ClassicCOMApplicationClasses";
            public const string ClassicCOMClass = "Win32_ClassicCOMClass";
            public const string ClassicCOMClassSetting = "Win32_ClassicCOMClassSetting";
            public const string ClassicCOMClassSettings = "Win32_ClassicCOMClassSettings";
            public const string ClassInforAction = "Win32_ClassInforAction";
            public const string ClientApplicationSetting = "Win32_ClientApplicationSetting";
            public const string CodecFile = "Win32_CodecFile";
            public const string COMApplicationSettings = "Win32_COMApplicationSettings";
            public const string COMClassAutoEmulator = "Win32_COMClassAutoEmulator";
            public const string ComClassEmulator = "Win32_ComClassEmulator";
            public const string CommandLineAccess = "Win32_CommandLineAccess";
            public const string ComponentCategory = "Win32_ComponentCategory";
            public const string ComputerSystem = "Win32_ComputerSystem";
            public const string ComputerSystemProcessor = "Win32_ComputerSystemProcessor";
            public const string ComputerSystemProduct = "Win32_ComputerSystemProduct";
            public const string ComputerSystemWindowsProductActivationSetting = "Win32_ComputerSystemWindowsProductActivationSetting";
            public const string Condition = "Win32_Condition";
            public const string ConnectionShare = "Win32_ConnectionShare";
            public const string ControllerHastHub = "Win32_ControllerHastHub";
            public const string CreateFolderAction = "Win32_CreateFolderAction";
            public const string CurrentProbe = "Win32_CurrentProbe";
            public const string DCOMApplication = "Win32_DCOMApplication";
            public const string DCOMApplicationAccessAllowedSetting = "Win32_DCOMApplicationAccessAllowedSetting";
            public const string DCOMApplicationLaunchAllowedSetting = "Win32_DCOMApplicationLaunchAllowedSetting";
            public const string DCOMApplicationSetting = "Win32_DCOMApplicationSetting";
            public const string DependentService = "Win32_DependentService";
            public const string Desktop = "Win32_Desktop";
            public const string DesktopMonitor = "Win32_DesktopMonitor";
            public const string DeviceBus = "Win32_DeviceBus";
            public const string DeviceMemoryAddress = "Win32_DeviceMemoryAddress";
            public const string Directory = "Win32_Directory";
            public const string DirectorySpecification = "Win32_DirectorySpecification";
            public const string DiskDrive = "Win32_DiskDrive";
            public const string DiskDrivePhysicalMedia = "Win32_DiskDrivePhysicalMedia";
            public const string DiskDriveToDiskPartition = "Win32_DiskDriveToDiskPartition";
            public const string DiskPartition = "Win32_DiskPartition";
            public const string DiskQuota = "Win32_DiskQuota";
            public const string DisplayConfiguration = "Win32_DisplayConfiguration";
            public const string DisplayControllerConfiguration = "Win32_DisplayControllerConfiguration";
            public const string DMAChanner = "Win32_DMAChanner";
            public const string DriverForDevice = "Win32_DriverForDevice";
            public const string DriverVXD = "Win32_DriverVXD";
            public const string DuplicateFileAction = "Win32_DuplicateFileAction";
            public const string Environment = "Win32_Environment";
            public const string EnvironmentSpecification = "Win32_EnvironmentSpecification";
            public const string ExtensionInfoAction = "Win32_ExtensionInfoAction";
            public const string Fan = "Win32_Fan";
            public const string FileSpecification = "Win32_FileSpecification";
            public const string FloppyController = "Win32_FloppyController";
            public const string FloppyDrive = "Win32_FloppyDrive";
            public const string FontInfoAction = "Win32_FontInfoAction";
            public const string Group = "Win32_Group";
            public const string GroupDomain = "Win32_GroupDomain";
            public const string GroupUser = "Win32_GroupUser";
            public const string HeatPipe = "Win32_HeatPipe";
            public const string IDEController = "Win32_IDEController";
            public const string IDEControllerDevice = "Win32_IDEControllerDevice";
            public const string ImplementedCategory = "Win32_ImplementedCategory";
            public const string InfraredDevice = "Win32_InfraredDevice";
            public const string IniFileSpecification = "Win32_IniFileSpecification";
            public const string InstalledSoftwareElement = "Win32_InstalledSoftwareElement";
            public const string IP4PersistedRouteTable = "Win32_IP4PersistedRouteTable";
            public const string IP4RouteTable = "Win32_IP4RouteTable";
            public const string IRQResource = "Win32_IRQResource";
            public const string Keyboard = "Win32_Keyboard";
            public const string LaunchCondition = "Win32_LaunchCondition";
            public const string LoadOrderGroup = "Win32_LoadOrderGroup";
            public const string LoadOrderGroupServiceDependencies = "Win32_LoadOrderGroupServiceDependencies";
            public const string LoadOrderGroupServiceMembers = "Win32_LoadOrderGroupServiceMembers";
            public const string LocalTime = "Win32_LocalTime";
            public const string LoggedOnUser = "Win32_LoggedOnUser";
            public const string LogicalDisk = "Win32_LogicalDisk";
            public const string LogicalDiskRootDirectory = "Win32_LogicalDiskRootDirectory";
            public const string LogicalDiskToPartition = "Win32_LogicalDiskToPartition";
            public const string LogicalFileAccess = "Win32_LogicalFileAccess";
            public const string LogicalFileAuditing = "Win32_LogicalFileAuditing";
            public const string LogicalFileGroup = "Win32_LogicalFileGroup";
            public const string LogicalFileOwner = "Win32_LogicalFileOwner";
            public const string LogicalFileSecuritySetting = "Win32_LogicalFileSecuritySetting";
            public const string LogicalMemoryConfiguration = "Win32_LogicalMemoryConfiguration";
            public const string LogicalProgramGroup = "Win32_LogicalProgramGroup";
            public const string LogicalProgramGroupDirectory = "Win32_LogicalProgramGroupDirectory";
            public const string LogicalProgramGroupItem = "Win32_LogicalProgramGroupItem";
            public const string LogicalProgramGroupItemDataFile = "Win32_LogicalProgramGroupItemDataFile";
            public const string LogicalShareAccess = "Win32_LogicalShareAccess";
            public const string LogicalShareAuditing = "Win32_LogicalShareAuditing";
            public const string LogicalShareSecuritySetting = "Win32_LogicalShareSecuritySetting";
            public const string LogonSession = "Win32_LogonSession";
            public const string LogonSessionMappedDisk = "Win32_LogonSessionMappedDisk";
            public const string MappedLogicalDisk = "Win32_MappedLogicalDisk";
            public const string MemoryArray = "Win32_MemoryArray";
            public const string MemoryArrayLocation = "Win32_MemoryArrayLocation";
            public const string MemoryDevice = "Win32_MemoryDevice";
            public const string MemoryDeviceArray = "Win32_MemoryDeviceArray";
            public const string MemoryDeviceLocation = "Win32_MemoryDeviceLocation";
            public const string MIMEInfoAction = "Win32_MIMEInfoAction";
            public const string MotherboardDevice = "Win32_MotherboardDevice";
            public const string MoveFileAction = "Win32_MoveFileAction";
            public const string NamedJobObject = "Win32_NamedJobObject";
            public const string NamedJobObjectActgInfo = "Win32_NamedJobObjectActgInfo";
            public const string NamedJobObjectLimit = "Win32_NamedJobObjectLimit";
            public const string NamedJobObjectLimitSetting = "Win32_NamedJobObjectLimitSetting";
            public const string NamedJobObjectProcess = "Win32_NamedJobObjectProcess";
            public const string NamedJobObjectSecLimit = "Win32_NamedJobObjectSecLimit";
            public const string NamedJobObjectSecLimitSetting = "Win32_NamedJobObjectSecLimitSetting";
            public const string NamedJobObjectStatistics = "Win32_NamedJobObjectStatistics";
            public const string NetworkAdapter = "Win32_NetworkAdapter";
            public const string NetworkAdapterConfiguration = "Win32_NetworkAdapterConfiguration";
            public const string NetworkAdapterSetting = "Win32_NetworkAdapterSetting";
            public const string NetworkClient = "Win32_NetworkClient";
            public const string NetworkConnection = "Win32_NetworkConnection";
            public const string NetworkLoginProfile = "Win32_NetworkLoginProfile";
            public const string NetworkProtocol = "Win32_NetworkProtocol";
            public const string NTDomain = "Win32_NTDomain";
            public const string NTEventlogFile = "Win32_NTEventlogFile";
            public const string NTLogEvent = "Win32_NTLogEvent";
            public const string NTLogEventComputer = "Win32_NTLogEventComputer";
            public const string NTLogEvnetLog = "Win32_NTLogEvnetLog";
            public const string NTLogEventUser = "Win32_NTLogEventUser";
            public const string ODBCAttribute = "Win32_ODBCAttribute";
            public const string ODBCDataSourceAttribute = "Win32_ODBCDataSourceAttribute";
            public const string ODBCDataSourceSpecification = "Win32_ODBCDataSourceSpecification";
            public const string ODBCDriverAttribute = "Win32_ODBCDriverAttribute";
            public const string ODBCDriverSoftwareElement = "Win32_ODBCDriverSoftwareElement";
            public const string ODBCDriverSpecification = "Win32_ODBCDriverSpecification";
            public const string ODBCSourceAttribute = "Win32_ODBCSourceAttribute";
            public const string ODBCTranslatorSpecification = "Win32_ODBCTranslatorSpecification";
            public const string OnBoardDevice = "Win32_OnBoardDevice";
            public const string OperatingSystem = "Win32_OperatingSystem";
            public const string OperatingSystemAutochkSetting = "Win32_OperatingSystemAutochkSetting";
            public const string OperatingSystemQFE = "Win32_OperatingSystemQFE";
            public const string OSRecoveryConfiguración = "Win32_OSRecoveryConfiguración";
            public const string PageFile = "Win32_PageFile";
            public const string PageFileElementSetting = "Win32_PageFileElementSetting";
            public const string PageFileSetting = "Win32_PageFileSetting";
            public const string PageFileUsage = "Win32_PageFileUsage";
            public const string ParallelPort = "Win32_ParallelPort";
            public const string Patch = "Win32_Patch";
            public const string PatchFile = "Win32_PatchFile";
            public const string PatchPackage = "Win32_PatchPackage";
            public const string PCMCIAControler = "Win32_PCMCIAControler";
            public const string PerfFormattedData_ASP_ActiveServerPages = "Win32_PerfFormattedData_ASP_ActiveServerPages";
            public const string PerfFormattedData_ASPNET_114322_ASPNETAppsv114322 = "Win32_PerfFormattedData_ASPNET_114322_ASPNETAppsv114322";
            public const string PerfFormattedData_ASPNET_114322_ASPNETv114322 = "Win32_PerfFormattedData_ASPNET_114322_ASPNETv114322";
            public const string PerfFormattedData_ASPNET_2040607_ASPNETAppsv2040607 = "Win32_PerfFormattedData_ASPNET_2040607_ASPNETAppsv2040607";
            public const string PerfFormattedData_ASPNET_2040607_ASPNETv2040607 = "Win32_PerfFormattedData_ASPNET_2040607_ASPNETv2040607";
            public const string PerfFormattedData_ASPNET_ASPNET = "Win32_PerfFormattedData_ASPNET_ASPNET";
            public const string PerfFormattedData_ASPNET_ASPNETApplications = "Win32_PerfFormattedData_ASPNET_ASPNETApplications";
            public const string PerfFormattedData_aspnet_state_ASPNETStateService = "Win32_PerfFormattedData_aspnet_state_ASPNETStateService";
            public const string PerfFormattedData_ContentFilter_IndexingServiceFilter = "Win32_PerfFormattedData_ContentFilter_IndexingServiceFilter";
            public const string PerfFormattedData_ContentIndex_IndexingService = "Win32_PerfFormattedData_ContentIndex_IndexingService";
            public const string PerfFormattedData_DTSPipeline_SQLServerDTSPipeline = "Win32_PerfFormattedData_DTSPipeline_SQLServerDTSPipeline";
            public const string PerfFormattedData_Fax_FaxServices = "Win32_PerfFormattedData_Fax_FaxServices";
            public const string PerfFormattedData_InetInfo_InternetInformationServicesGlobal = "Win32_PerfFormattedData_InetInfo_InternetInformationServicesGlobal";
            public const string PerfFormattedData_ISAPISearch_HttpIndexingService = "Win32_PerfFormattedData_ISAPISearch_HttpIndexingService";
            public const string PerfFormattedData_MSDTC_DistributedTransactionCoordinator = "Win32_PerfFormattedData_MSDTC_DistributedTransactionCoordinator";
            public const string PerfFormattedData_NETCLRData_NETCLRData = "Win32_PerfFormattedData_NETCLRData_NETCLRData";
            public const string PerfFormattedData_NETCLRNetworking_NETCLRNetworking = "Win32_PerfFormattedData_NETCLRNetworking_NETCLRNetworking";
            public const string PerfFormattedData_NETDataProviderforOracle_NETCLRData = "Win32_PerfFormattedData_NETDataProviderforOracle_NETCLRData";
            public const string PerfFormattedData_NETDataProviderforSqlServer_NETDataProviderforSqlServer = "Win32_PerfFormattedData_NETDataProviderforSqlServer_NETDataProviderforSqlServer";
            public const string PerfFormattedData_NETFramework_NETCLRExceptions = "Win32_PerfFormattedData_NETFramework_NETCLRExceptions";
            public const string PerfFormattedData_NETFramework_NETCLRInterop = "Win32_PerfFormattedData_NETFramework_NETCLRInterop";
            public const string PerfFormattedData_NETFramework_NETCLRJit = "Win32_PerfFormattedData_NETFramework_NETCLRJit";
            public const string PerfFormattedData_NETFramework_NETCLRLoading = "Win32_PerfFormattedData_NETFramework_NETCLRLoading";
            public const string PerfFormattedData_NETFramework_NETCLRLocksAndThreads = "Win32_PerfFormattedData_NETFramework_NETCLRLocksAndThreads";
            public const string PerfFormattedData_NETFramework_NETCLRMemory = "Win32_PerfFormattedData_NETFramework_NETCLRMemory";
            public const string PerfFormattedData_NETFramework_NETCLRRemoting = "Win32_PerfFormattedData_NETFramework_NETCLRRemoting";
            public const string PerfFormattedData_NETFramework_NETCLRSecurity = "Win32_PerfFormattedData_NETFramework_NETCLRSecurity";
            public const string PerfFormattedData_NTFSDRV_ControladordealmacenamientoNTFSdeSMTP = "Win32_PerfFormattedData_NTFSDRV_ControladordealmacenamientoNTFSdeSMTP";
            public const string PerfFormattedData_Outlook_Outlook = "Win32_PerfFormattedData_Outlook_Outlook";
            public const string PerfFormattedData_PerfDisk_LogicalDisk = "Win32_PerfFormattedData_PerfDisk_LogicalDisk";
            public const string PerfFormattedData_PerfDisk_PhysicalDisk = "Win32_PerfFormattedData_PerfDisk_PhysicalDisk";
            public const string PerfFormattedData_PerfNet_Browser = "Win32_PerfFormattedData_PerfNet_Browser";
            public const string PerfFormattedData_PerfNet_Redirector = "Win32_PerfFormattedData_PerfNet_Redirector";
            public const string PerfFormattedData_PerfNet_Server = "Win32_PerfFormattedData_PerfNet_Server";
            public const string PerfFormattedData_PerfNet_ServerWorkQueues = "Win32_PerfFormattedData_PerfNet_ServerWorkQueues";
            public const string PerfFormattedData_PerfOS_Cache = "Win32_PerfFormattedData_PerfOS_Cache";
            public const string PerfFormattedData_PerfOS_Memory = "Win32_PerfFormattedData_PerfOS_Memory";
            public const string PerfFormattedData_PerfOS_Objects = "Win32_PerfFormattedData_PerfOS_Objects";
            public const string PerfFormattedData_PerfOS_PagingFile = "Win32_PerfFormattedData_PerfOS_PagingFile";
            public const string PerfFormattedData_PerfOS_Processor = "Win32_PerfFormattedData_PerfOS_Processor";
            public const string PerfFormattedData_PerfOS_System = "Win32_PerfFormattedData_PerfOS_System";
            public const string PerfFormattedData_PerfProc_FullImage_Costly = "Win32_PerfFormattedData_PerfProc_FullImage_Costly";
            public const string PerfFormattedData_PerfProc_Image_Costly = "Win32_PerfFormattedData_PerfProc_Image_Costly";
            public const string PerfFormattedData_PerfProc_JobObject = "Win32_PerfFormattedData_PerfProc_JobObject";
            public const string PerfFormattedData_PerfProc_JobObjectDetails = "Win32_PerfFormattedData_PerfProc_JobObjectDetails";
            public const string PerfFormattedData_PerfProc_Process = "Win32_PerfFormattedData_PerfProc_Process";
            public const string PerfFormattedData_PerfProc_ProcessAddressSpace_Costly = "Win32_PerfFormattedData_PerfProc_ProcessAddressSpace_Costly";
            public const string PerfFormattedData_PerfProc_Thread = "Win32_PerfFormattedData_PerfProc_Thread";
            public const string PerfFormattedData_PerfProc_ThreadDetails_Costly = "Win32_PerfFormattedData_PerfProc_ThreadDetails_Costly";
            public const string PerfFormattedData_RemoteAccess_RASPort = "Win32_PerfFormattedData_RemoteAccess_RASPort";
            public const string PerfFormattedData_RemoteAccess_RASTotal = "Win32_PerfFormattedData_RemoteAccess_RASTotal";
            public const string PerfFormattedData_RSVP_RSVPInterfaces = "Win32_PerfFormattedData_RSVP_RSVPInterfaces";
            public const string PerfFormattedData_RSVP_RSVPService = "Win32_PerfFormattedData_RSVP_RSVPService";
            public const string PerfFormattedData_Spooler_PrintQueue = "Win32_PerfFormattedData_Spooler_PrintQueue";
            public const string PerfFormattedData_TapiSrv_Telephony = "Win32_PerfFormattedData_TapiSrv_Telephony";
            public const string PerfFormattedData_Tcpip_ICMP = "Win32_PerfFormattedData_Tcpip_ICMP";
            public const string PerfFormattedData_Tcpip_IP = "Win32_PerfFormattedData_Tcpip_IP";
            public const string PerfFormattedData_Tcpip_NBTConnection = "Win32_PerfFormattedData_Tcpip_NBTConnection";
            public const string PerfFormattedData_Tcpip_NetworkInterface = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            public const string PerfFormattedData_Tcpip_TCP = "Win32_PerfFormattedData_Tcpip_TCP";
            public const string PerfFormattedData_Tcpip_UDP = "Win32_PerfFormattedData_Tcpip_UDP";
            public const string PerfFormattedData_TermService_TerminalServices = "Win32_PerfFormattedData_TermService_TerminalServices";
            public const string PerfFormattedData_TermService_TerminalServicesSession = "Win32_PerfFormattedData_TermService_TerminalServicesSession";
            public const string PerfFormattedData_W3SVC_WebService = "Win32_PerfFormattedData_W3SVC_WebService";
            public const string PerfRawData_ASP_ActiveServerPages = "Win32_PerfRawData_ASP_ActiveServerPages";
            public const string PerfRawData_ASPNET_114322_ASPNETAppsv114322 = "Win32_PerfRawData_ASPNET_114322_ASPNETAppsv114322";
            public const string PerfRawData_ASPNET_114322_ASPNETv114322 = "Win32_PerfRawData_ASPNET_114322_ASPNETv114322";
            public const string PerfRawData_ASPNET_2040607_ASPNETAppsv2040607 = "Win32_PerfRawData_ASPNET_2040607_ASPNETAppsv2040607";
            public const string PerfRawData_ASPNET_2040607_ASPNETv2040607 = "Win32_PerfRawData_ASPNET_2040607_ASPNETv2040607";
            public const string PerfRawData_ASPNET_ASPNET = "Win32_PerfRawData_ASPNET_ASPNET";
            public const string PerfRawData_ASPNET_ASPNETApplications = "Win32_PerfRawData_ASPNET_ASPNETApplications";
            public const string PerfRawData_aspnet_state_ASPNETStateService = "Win32_PerfRawData_aspnet_state_ASPNETStateService";
            public const string PerfRawData_ContentFilter_IndexingServiceFilter = "Win32_PerfRawData_ContentFilter_IndexingServiceFilter";
            public const string PerfRawData_ContentIndex_IndexingService = "Win32_PerfRawData_ContentIndex_IndexingService";
            public const string PerfRawData_DTSPipeline_SQLServerDTSPipeline = "Win32_PerfRawData_DTSPipeline_SQLServerDTSPipeline";
            public const string PerfRawData_Fax_FaxServices = "Win32_PerfRawData_Fax_FaxServices";
            public const string PerfRawData_InetInfo_InternetInformationServicesGlobal = "Win32_PerfRawData_InetInfo_InternetInformationServicesGlobal";
            public const string PerfRawData_ISAPISearch_HttpIndexingService = "Win32_PerfRawData_ISAPISearch_HttpIndexingService";
            public const string PerfRawData_MSDTC_DistributedTransactionCoordinator = "Win32_PerfRawData_MSDTC_DistributedTransactionCoordinator";
            public const string PerfRawData_NETCLRData_NETCLRData = "Win32_PerfRawData_NETCLRData_NETCLRData";
            public const string PerfRawData_NETCLRNetworking_NETCLRNetworking = "Win32_PerfRawData_NETCLRNetworking_NETCLRNetworking";
            public const string PerfRawData_NETDataProviderforOracle_NETCLRData = "Win32_PerfRawData_NETDataProviderforOracle_NETCLRData";
            public const string PerfRawData_NETDataProviderforSqlServer_NETDataProviderforSqlServer = "Win32_PerfRawData_NETDataProviderforSqlServer_NETDataProviderforSqlServer";
            public const string PerfRawData_NETFramework_NETCLRExceptions = "Win32_PerfRawData_NETFramework_NETCLRExceptions";
            public const string PerfRawData_NETFramework_NETCLRInterop = "Win32_PerfRawData_NETFramework_NETCLRInterop";
            public const string PerfRawData_NETFramework_NETCLRJit = "Win32_PerfRawData_NETFramework_NETCLRJit";
            public const string PerfRawData_NETFramework_NETCLRLoading = "Win32_PerfRawData_NETFramework_NETCLRLoading";
            public const string PerfRawData_NETFramework_NETCLRLocksAndThreads = "Win32_PerfRawData_NETFramework_NETCLRLocksAndThreads";
            public const string PerfRawData_NETFramework_NETCLRMemory = "Win32_PerfRawData_NETFramework_NETCLRMemory";
            public const string PerfRawData_NETFramework_NETCLRRemoting = "Win32_PerfRawData_NETFramework_NETCLRRemoting";
            public const string PerfRawData_NETFramework_NETCLRSecurity = "Win32_PerfRawData_NETFramework_NETCLRSecurity";
            public const string PerfRawData_NTFSDRV_ControladordealmacenamientoNTFSdeSMTP = "Win32_PerfRawData_NTFSDRV_ControladordealmacenamientoNTFSdeSMTP";
            public const string PerfRawData_Outlook_Outlook = "Win32_PerfRawData_Outlook_Outlook";
            public const string PerfRawData_PerfDisk_LogicalDisk = "Win32_PerfRawData_PerfDisk_LogicalDisk";
            public const string PerfRawData_PerfDisk_PhysicalDisk = "Win32_PerfRawData_PerfDisk_PhysicalDisk";
            public const string PerfRawData_PerfNet_Browser = "Win32_PerfRawData_PerfNet_Browser";
            public const string PerfRawData_PerfNet_Redirector = "Win32_PerfRawData_PerfNet_Redirector";
            public const string PerfRawData_PerfNet_Server = "Win32_PerfRawData_PerfNet_Server";
            public const string PerfRawData_PerfNet_ServerWorkQueues = "Win32_PerfRawData_PerfNet_ServerWorkQueues";
            public const string PerfRawData_PerfOS_Cache = "Win32_PerfRawData_PerfOS_Cache";
            public const string PerfRawData_PerfOS_Memory = "Win32_PerfRawData_PerfOS_Memory";
            public const string PerfRawData_PerfOS_Objects = "Win32_PerfRawData_PerfOS_Objects";
            public const string PerfRawData_PerfOS_PagingFile = "Win32_PerfRawData_PerfOS_PagingFile";
            public const string PerfRawData_PerfOS_Processor = "Win32_PerfRawData_PerfOS_Processor";
            public const string PerfRawData_PerfOS_System = "Win32_PerfRawData_PerfOS_System";
            public const string PerfRawData_PerfProc_FullImage_Costly = "Win32_PerfRawData_PerfProc_FullImage_Costly";
            public const string PerfRawData_PerfProc_Image_Costly = "Win32_PerfRawData_PerfProc_Image_Costly";
            public const string PerfRawData_PerfProc_JobObject = "Win32_PerfRawData_PerfProc_JobObject";
            public const string PerfRawData_PerfProc_JobObjectDetails = "Win32_PerfRawData_PerfProc_JobObjectDetails";
            public const string PerfRawData_PerfProc_Process = "Win32_PerfRawData_PerfProc_Process";
            public const string PerfRawData_PerfProc_ProcessAddressSpace_Costly = "Win32_PerfRawData_PerfProc_ProcessAddressSpace_Costly";
            public const string PerfRawData_PerfProc_Thread = "Win32_PerfRawData_PerfProc_Thread";
            public const string PerfRawData_PerfProc_ThreadDetails_Costly = "Win32_PerfRawData_PerfProc_ThreadDetails_Costly";
            public const string PerfRawData_RemoteAccess_RASPort = "Win32_PerfRawData_RemoteAccess_RASPort";
            public const string PerfRawData_RemoteAccess_RASTotal = "Win32_PerfRawData_RemoteAccess_RASTotal";
            public const string PerfRawData_RSVP_RSVPInterfaces = "Win32_PerfRawData_RSVP_RSVPInterfaces";
            public const string PerfRawData_RSVP_RSVPService = "Win32_PerfRawData_RSVP_RSVPService";
            public const string PerfRawData_Spooler_PrintQueue = "Win32_PerfRawData_Spooler_PrintQueue";
            public const string PerfRawData_TapiSrv_Telephony = "Win32_PerfRawData_TapiSrv_Telephony";
            public const string PerfRawData_Tcpip_ICMP = "Win32_PerfRawData_Tcpip_ICMP";
            public const string PerfRawData_Tcpip_IP = "Win32_PerfRawData_Tcpip_IP";
            public const string PerfRawData_Tcpip_NBTConnection = "Win32_PerfRawData_Tcpip_NBTConnection";
            public const string PerfRawData_Tcpip_NetworkInterface = "Win32_PerfRawData_Tcpip_NetworkInterface";
            public const string PerfRawData_Tcpip_TCP = "Win32_PerfRawData_Tcpip_TCP";
            public const string PerfRawData_Tcpip_UDP = "Win32_PerfRawData_Tcpip_UDP";
            public const string PerfRawData_TermService_TerminalServices = "Win32_PerfRawData_TermService_TerminalServices";
            public const string PerfRawData_TermService_TerminalServicesSession = "Win32_PerfRawData_TermService_TerminalServicesSession";
            public const string PerfRawData_W3SVC_WebService = "Win32_PerfRawData_W3SVC_WebService";
            public const string PhysicalMedia = "Win32_PhysicalMedia";
            public const string PhysicalMemory = "Win32_PhysicalMemory";
            public const string PhysicalMemoryArray = "Win32_PhysicalMemoryArray";
            public const string PhysicalMemoryLocation = "Win32_PhysicalMemoryLocation";
            public const string PingStatus = "Win32_PingStatus";
            public const string PNPAllocatedResource = "Win32_PNPAllocatedResource";
            public const string PnPDevice = "Win32_PnPDevice";
            public const string PnPEntity = "Win32_PnPEntity";
            public const string PnPSignedDriver = "Win32_PnPSignedDriver";
            public const string PnPSignedDriverCIMDataFile = "Win32_PnPSignedDriverCIMDataFile";
            public const string PointingDevice = "Win32_PointingDevice";
            public const string PortableBattery = "Win32_PortableBattery";
            public const string PortConnector = "Win32_PortConnector";
            public const string PortResource = "Win32_PortResource";
            public const string POTSModem = "Win32_POTSModem";
            public const string POTSModemToSerialPort = "Win32_POTSModemToSerialPort";
            public const string Printer = "Win32_Printer";
            public const string PrinterConfiguration = "Win32_PrinterConfiguration";
            public const string PrinterController = "Win32_PrinterController";
            public const string PrinterDriver = "Win32_PrinterDriver";
            public const string PrinterDriverDll = "Win32_PrinterDriverDll";
            public const string PrinterSetting = "Win32_PrinterSetting";
            public const string PrinterShare = "Win32_PrinterShare";
            public const string PrintJob = "Win32_PrintJob";
            public const string Process = "Win32_Process";
            public const string Processor = "Win32_Processor";
            public const string Product = "Win32_Product";
            public const string ProductCheck = "Win32_ProductCheck";
            public const string ProductResource = "Win32_ProductResource";
            public const string ProductSoftwareFeatures = "Win32_ProductSoftwareFeatures";
            public const string ProgIDSpecification = "Win32_ProgIDSpecification";
            public const string ProgramGroup = "Win32_ProgramGroup";
            public const string ProgramGroupContents = "Win32_ProgramGroupContents";
            public const string Property = "Win32_Property";
            public const string ProtocolBinding = "Win32_ProtocolBinding";
            public const string Proxy = "Win32_Proxy";
            public const string PublishComponentAction = "Win32_PublishComponentAction";
            public const string QuickFixEngineering = "Win32_QuickFixEngineering";
            public const string QuotaSetting = "Win32_QuotaSetting";
            public const string Refrigeration = "Win32_Refrigeration";
            public const string Registry = "Win32_Registry";
            public const string RegistryAction = "Win32_RegistryAction";
            public const string RemoveFileAction = "Win32_RemoveFileAction";
            public const string RemoveIniAction = "Win32_RemoveIniAction";
            public const string ReserveCost = "Win32_ReserveCost";
            public const string ScheduledJob = "Win32_ScheduledJob";
            public const string SCSIController = "Win32_SCSIController";
            public const string SCSIControllerDevice = "Win32_SCSIControllerDevice";
            public const string SecuritySettingOfLogicalFile = "Win32_SecuritySettingOfLogicalFile";
            public const string SecuritySettingOfLogicalShare = "Win32_SecuritySettingOfLogicalShare";
            public const string SelfRegModuleAction = "Win32_SelfRegModuleAction";
            public const string SerialPort = "Win32_SerialPort";
            public const string SerialPortConfiguration = "Win32_SerialPortConfiguration";
            public const string SerialPortSetting = "Win32_SerialPortSetting";
            public const string ServerConnection = "Win32_ServerConnection";
            public const string ServerSession = "Win32_ServerSession";
            public const string Service = "Win32_Service";
            public const string ServiceControl = "Win32_ServiceControl";
            public const string ServiceSpecification = "Win32_ServiceSpecification";
            public const string ServiceSpecificationService = "Win32_ServiceSpecificationService";
            public const string SessionConnection = "Win32_SessionConnection";
            public const string SessionProcess = "Win32_SessionProcess";
            public const string Share = "Win32_Share";
            public const string ShareToDirectory = "Win32_ShareToDirectory";
            public const string ShortcutAction = "Win32_ShortcutAction";
            public const string ShortcutFile = "Win32_ShortcutFile";
            public const string ShortcutSAP = "Win32_ShortcutSAP";
            public const string SID = "Win32_SID";
            public const string SoftwareElement = "Win32_SoftwareElement";
            public const string SoftwareElementAction = "Win32_SoftwareElementAction";
            public const string SoftwareElementCheck = "Win32_SoftwareElementCheck";
            public const string SoftwareElementCondition = "Win32_SoftwareElementCondition";
            public const string SoftwareElementResource = "Win32_SoftwareElementResource";
            public const string SoftwareFeature = "Win32_SoftwareFeature";
            public const string SoftwareFeatureAction = "Win32_SoftwareFeatureAction";
            public const string SoftwareFeatureCheck = "Win32_SoftwareFeatureCheck";
            public const string SoftwareFeatureParent = "Win32_SoftwareFeatureParent";
            public const string SoftwareFeatureSoftwareElements = "Win32_SoftwareFeatureSoftwareElements";
            public const string SoundDevice = "Win32_SoundDevice";
            public const string StartupCommand = "Win32_StartupCommand";
            public const string SubDirectory = "Win32_SubDirectory";
            public const string SystemAccount = "Win32_SystemAccount";
            public const string SystemBIOS = "Win32_SystemBIOS";
            public const string SystemBootConfiguration = "Win32_SystemBootConfiguration";
            public const string SystemDesktop = "Win32_SystemDesktop";
            public const string SystemDevices = "Win32_SystemDevices";
            public const string SystemDriver = "Win32_SystemDriver";
            public const string SystemDriverPNPEntity = "Win32_SystemDriverPNPEntity";
            public const string SystemEnclosure = "Win32_SystemEnclosure";
            public const string SystemLoadOrderGroups = "Win32_SystemLoadOrderGroups";
            public const string SystemLogicalMemoryConfiguration = "Win32_SystemLogicalMemoryConfiguration";
            public const string SystemNetworkConnections = "Win32_SystemNetworkConnections";
            public const string SystemOperatingSystem = "Win32_SystemOperatingSystem";
            public const string SystemPartitions = "Win32_SystemPartitions";
            public const string SystemProcesses = "Win32_SystemProcesses";
            public const string SystemProgramGroups = "Win32_SystemProgramGroups";
            public const string SystemResources = "Win32_SystemResources";
            public const string SystemServices = "Win32_SystemServices";
            public const string SystemSlot = "Win32_SystemSlot";
            public const string SystemSystemDriver = "Win32_SystemSystemDriver";
            public const string SystemTimeZone = "Win32_SystemTimeZone";
            public const string SystemUsers = "Win32_SystemUsers";
            public const string TapeDrive = "Win32_TapeDrive";
            public const string TCPIPPrinterPort = "Win32_TCPIPPrinterPort";
            public const string TemperatureProbe = "Win32_TemperatureProbe";
            public const string Terminal = "Win32_Terminal";
            public const string TerminalService = "Win32_TerminalService";
            public const string TerminalServiceSetting = "Win32_TerminalServiceSetting";
            public const string TerminalServiceToSetting = "Win32_TerminalServiceToSetting";
            public const string TerminalTerminalSetting = "Win32_TerminalTerminalSetting";
            public const string Thread = "Win32_Thread";
            public const string TimeZone = "Win32_TimeZone";
            public const string TSAccount = "Win32_TSAccount";
            public const string TSClientSetting = "Win32_TSClientSetting";
            public const string TSEnvironmentSetting = "Win32_TSEnvironmentSetting";
            public const string TSGeneralSetting = "Win32_TSGeneralSetting";
            public const string TSLogonSetting = "Win32_TSLogonSetting";
            public const string TSNetworkAdapterListSetting = "Win32_TSNetworkAdapterListSetting";
            public const string TSNetworkAdapterSetting = "Win32_TSNetworkAdapterSetting";
            public const string TSPermissionsSetting = "Win32_TSPermissionsSetting";
            public const string TSRemoteControlSetting = "Win32_TSRemoteControlSetting";
            public const string TSSessionDirectory = "Win32_TSSessionDirectory";
            public const string TSSessionDirectorySetting = "Win32_TSSessionDirectorySetting";
            public const string TSSessionSetting = "Win32_TSSessionSetting";
            public const string TypeLibraryAction = "Win32_TypeLibraryAction";
            public const string UninterruptiblePowerSupply = "Win32_UninterruptiblePowerSupply";
            public const string USBController = "Win32_USBController";
            public const string USBControllerDevice = "Win32_USBControllerDevice";
            public const string USBHub = "Win32_USBHub";
            public const string UserAccount = "Win32_UserAccount";
            public const string UserDesktop = "Win32_UserDesktop";
            public const string UserInDomain = "Win32_UserInDomain";
            public const string UTCTime = "Win32_UTCTime";
            public const string VideoController = "Win32_VideoController";
            public const string VideoSettings = "Win32_VideoSettings";
            public const string VoltageProbe = "Win32_VoltageProbe";
            public const string VolumeQuotaSetting = "Win32_VolumeQuotaSetting";
            public const string WindowsProductActivation = "Win32_WindowsProductActivation";
            public const string WMIElementSetting = "Win32_WMIElementSetting";
            public const string WMISetting = "Win32_WMISetting";
        }

        volatile static SysInformation _instance;
        readonly Dictionary<string, Dictionary<string, object>> _infoCache;

        #region constructor / destructor

        private SysInformation()
        {
            _infoCache = new Dictionary<string, Dictionary<string, object>>();
        }

        ~SysInformation()
        {
            Clear();
        }

        #endregion

        #region properties

        public static SysInformation Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SysInformation();

                return _instance;
            }
        }

        public object this[string className, string property]
        {
            get
            {
                if (_infoCache.ContainsKey(className) && _infoCache[className].ContainsKey(property))
                    return _infoCache[className][property];

                return null;
            }
        }

        #endregion

        public void Clear()
        {
            _infoCache.Clear();
        }

        public void Fill(string className, bool reload = false)
        {
            if (_infoCache.ContainsKey(className))
            {
                if (!reload)
                    return;
            }
            else
                _infoCache.Add(className, new Dictionary<string, object>());

            using (var managementClass = new ManagementClass(className))
            {
                var properties = managementClass.Properties;
                using (var managementObjects = managementClass.GetInstances())
                {
                    foreach (var managementObject in managementObjects)
                    {
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (_infoCache[className].ContainsKey(property.Name))
                                    _infoCache[className][property.Name] = managementObject.Properties[property.Name].Value;
                                else
                                    _infoCache[className].Add(property.Name, managementObject.Properties[property.Name].Value);
                            }
                            catch
                            {
                                // do nothing here
                            }
                            
                        }
                    }
                }
            }
            
        }
    }
}
