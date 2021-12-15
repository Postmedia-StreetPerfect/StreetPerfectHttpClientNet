#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
//using System.Text.Json.Serialization;

// just street perfect models used by StreetPerfect.Client - and the site
// models represent the input (request) and the output (response) parameters of
// the low level spaa calls
namespace StreetPerfect.Models
{
	public class SPConst
	{
		public const string DataNamespace = "http://StreetPerfect.com/wcf/models/";
		public const string ServiceNamespace = "http://StreetPerfect.com/wcf/";
	}

	public class TokenResponse
	{
		/// <summary>
		/// The actual JWT, set this to the Authorization http header for all subsequent requests
		/// 
		/// Authorization: Bearer [AccessToken]
		/// </summary>
		public string AccessToken { get; set; }

		/// <summary>
		/// Token type, should always be 'Bearer'
		/// </summary>
		public string TokenType { get; set; }

		/// <summary>
		/// The refresh token to save for refreshing via api/token/refresh endpoint
		/// </summary>
		public string RefreshToken { get; set; }

		/// <summary>
		/// The number of minutes this token will take to expire.
		/// After that you must refresh the token.
		/// </summary>
		public int Expires { get; set; }

		/// <summary>
		/// The date (UTC) that this token will NOT refresh anymore. 
		/// 
		/// If the token refresh never expires then this will be null or simply missing.
		/// </summary>
		public DateTime? RefreshExpireDate { get; set; }

		/// <summary>
		/// error message or 'OK' if ok...
		/// </summary>
		public string Msg { get; set; }
	}

	public class TokenRequest
	{
		/// <summary>
		/// Your client id - normally the email address you use to logon the site.
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// This is the generated api key you must store safely.
		/// </summary>
		public string ClientSecret { get; set; }
	}

	public class TokenRefreshRequest
	{
		/// <summary>
		/// The JWT access token
		/// </summary>
		public string AccessToken { get; set; }

		/// <summary>
		/// The refresh token that was sent via the new token endpoint or the last refresh.
		/// </summary>
		public string RefreshToken { get; set; }
	}




	[DataContract(Namespace = SPConst.DataNamespace)]
	public class Options
	{
		/// <summary>
		/// 
		/// PreferredLanguageStyle= [ [I]NPUT | [E]NGLISH | [F]RENCH | [C]PCDB ]
		/// </summary>
		[DataMember]
		public string PreferredLanguageStyle { get; set; }



		/// <summary>
		/// Users preferred language for messages and reporting.
		/// 
		/// UserLanguage= [E]nglish, [F]rench
		/// </summary>
		[DataMember]
		public string UserLanguage { get; set; }



		/// <summary>
		/// Controls what type of message codes are returned with the correction messages.
		/// 
		/// Defaults to 'N'
		/// 
		/// 'Y' - returns a 3 digit prefix to address correction messages for
		/// programmatic use. These may be used to provide a filtering
		/// capability. Message codes can be found in the DocumentFiles
		/// installation folder.
		/// 
		/// 'N' - returns a 3 character prefix to address correction messages
		/// indicating message class. For Canadian product only, these
		/// prefixes are:
		/// 
		/// INP - Original input line
		/// 
		/// INF - Informational message
		/// 
		/// OPT - Optimization message
		/// 
		/// CHG - Change message
		/// 
		/// TRY - Try message - engine has identified one or more possibilities as a potential correction but there was insufficient data or ambiguous results making identification of a correction unreliable.
		/// 
		/// ERR - Error message
		/// | Value | Print Message Class | Return Message Class | Print Message Numbers | Return Message Numbers |
		/// |-------|---------------------|----------------------|-----------------------|------------------------|
		/// |  N/0  |         N           |           N          |           N           |            N           | 
		/// |  Y/1  |         N           |           N          |           Y           |            N           | 
		/// |    2  |         N           |           N          |           N           |            Y           | 
		/// |    3  |         N           |           N          |           Y           |            Y           | 
		/// |    4  |         N           |           Y          |           N           |            Y           | 
		/// |    5  |         N           |           Y          |           Y           |            Y           | 
		/// |    6  |         Y           |           Y          |           Y           |            Y           | 
		/// 
		/// 
		/// (used in Correct, Validate)
		/// 
		/// </summary>
		[DataMember]
		public string PrintMessageNumbers { get; set; }


		/// <summary>
		/// Maximum Tries Flag
		/// - The maximum number of possible alternate addresses to print if unable to correct.
		/// - These alternate addresses will appear on the exception report.
		/// </summary>
		public int? MaximumTryMessages { get; set; }



		/// <summary>
		/// Error Tolerance Indicator
		/// - Determines how “closely” an input address must come to a Canada Post address to be considered a match.
		/// - The value indicates the number of components that may be in variance.
		/// - Allowed values; 0 - 4 
		/// </summary>
		public int? ErrorTolerance { get; set; }



		/// <summary>
		/// Defaults to 'SUITE'
		/// 
		/// | English Abbr. | French Abbr. | English Full Name | French Full Name |
		/// |---------------|--------------|-------------------|------------------|
		/// | ‘UNIT’        | ‘UNITE’      | ‘UNIT’            | ‘UNITE’          |
		/// | ‘APT’         | ‘APP’        | ‘APARTMENT’       | ‘APPARTMENT’ |
		/// | ‘SUITE’       | ‘BUREAU’     | ‘SUITE’           | ‘BUREAU’ |
		/// | ‘TH’          |              | ‘TOWNHOUSE’ |
		/// | ‘TWNHSE’      |              | ‘TOWNHOUSE’ |
		/// | ‘RM’          |              | ‘ROOM’  |
		/// | ‘PH’          |              | ‘PENTHOUSE’  |
		/// | ‘PIECE’       |              |                   | ‘PIECE’  |
		/// | ‘SALLE’       |              |                   | ‘SALLE’ |
		/// 
		/// 
		/// (used in Correct, Format, Fetch)
		/// </summary>
		[DataMember]
		public string PreferredUnitDesignatorKeyword { get; set; }


		/// <summary>
		/// Defaults to 'K'
		/// 
		/// 'K' Keyword Style
		/// 
		/// e.g. 123 MAIN ST SUITE 5
		/// 
		/// 'W' Western Style
		/// 
		/// e.g. 5-123 MAIN ST
		/// 
		/// (used in Correct, Format, Fetch)
		/// </summary>
		[DataMember]
		public string PreferredUnitDesignatorStyle { get; set; }


		/// <summary>
		/// Defaults to 'N'
		/// 
		/// 'N' - Natural
		/// Follows the language rules:
		/// - Street type placement relative to street name.
		/// - English types always follow street names.
		/// - Upper case
		///  
		/// 'S' - Street
		/// - Forces the street name to always appear first
		/// - Upper case
		/// 
		/// Alternate values
		/// 1. Same as N +
		/// 2. Same as S +
		/// 3. Same as N with upper case French accents +
		/// 4. Same as S with upper case French accents +
		/// 5. Same as N with mixed case +
		/// 6. Same as S with mixed case +
		/// 7. Same as N with mixed case French accents +
		/// 8. Same as S with mixed case French accents +
		/// 
		/// ( + Only accessible from the Format function )
		/// 
		/// (used in Correct, Format, Fetch)
		/// </summary>
		[DataMember]
		public string OutputFormatGuide { get; set; }


		/// <summary>
		/// Defaults to 'false'
		/// 
		/// 'true' - Yes, print information messages in exception report.
		/// 
		/// This includes items from the Canada Post LVR (Text 4) file.
		/// 
		/// 'false' - No, do not print information messages.
		/// 
		/// (used in Correct, Validate)
		/// </summary>
		[DataMember]
		public bool? PrintInformationMessages { get; set; }


		/// <summary>
		/// Defaults to 'true'
		/// 
		/// 'true' - print change messages in exception report.
		/// - This will identify changes between the old and corrected address.
		/// 
		/// 'false' - do not print change messages.
		/// 
		/// (used in Correct, Validate)
		/// </summary>
		[DataMember]
		public bool? PrintChangeMessages { get; set; }


		/// <summary>
		/// Defaults to 'true'
		/// 
		/// 'true' - print error messages in exception report.
		/// - This will document specific address errors.
		/// 
		/// 'false' - do not print error messages.
		/// 
		/// (used in Correct, Validate)
		/// </summary>
		[DataMember]
		public bool? PrintErrorMessages { get; set; }


		/// <summary>
		/// Defaults to 'true'
		/// 
		/// 'true' - print try messages in exception report. This will list the best possible address corrections up to the maximum tries (SB_IN_PRINT_MAX_TRY_MESS)
		/// 
		/// 'false' - do not print try messages.
		/// 
		/// (used in Correct, Validate)
		/// </summary>
		[DataMember]
		public bool? PrintTryMessages { get; set; }


		/// <summary>
		/// Defaults to 'D'
		/// 
		/// 'D' Detail Report - Print detail messages.
		/// 
		/// 'S' Summary Report - Print summary only.
		/// 
		/// 'E' Error Report - Print errors only.
		/// 
		/// 'N' No Report
		/// 
		/// (used in Correct, Validate)
		/// </summary>
		[DataMember]
		public string ExceptionReportLevel { get; set; }


		/// <summary>
		/// Defaults to 'true'
		/// 
		/// 'true' - print optimization messages in exception report. This will list optimization performed on the address. This occurs only if Optimum Address Flag (OptimizeAddress) = 'Y'
		/// 
		/// 'false' - do not print optimization messages.
		/// 
		/// (used in Correct)
		/// </summary>
		[DataMember]
		public bool? PrintOptimizeMessages { get; set; }


		/// <summary>		
		/// 'Y' - replace address components with Canada Post symbols where possible.
		/// 
		/// E.g. 123 KING STREET EAST Input
		/// 
		/// 123 KING ST E Output
		/// 
		/// 'N' - No, do not optimize address to Canada Post symbols.
		/// 
		/// 'S' - Standardize, perform a series of possible changes to the input address including:
		/// 
		/// - convert of all CPC keywords to symbols
		/// - remove Extra Info and Unidentified Components from address line
		/// - change input municipality name to CPC database municipality name
		/// - format unit / apt / suite information according to PreferredUnitDesignatorStyle keyword
		/// - The Correct and Parse APIs will honour the OutputFormatGuide parameter so that it is possible to receive mixed case or French accented data results from these APIs. 
		/// 
		/// (used in Correct)
		/// </summary>
		[DataMember]
		public string OptimizeAddress { get; set; }


	}




	/// <summary>
	/// used for both Connect and Disconnect 
	/// ONLY for use with the SPC (single threaded network session) client library
	/// </summary>
	[DataContract(Namespace =SPConst.DataNamespace)]
	public class ConnectionResponse
	{

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

	}

	/* ca field names only
		Id
		rec_typ_cde
		adr_typ_cde
		prov_cde
		drctry_area_nme
		st_nme
		st_typ_cde
		st_drctn_cde
		st_adr_seq_cde
		st_adr_to_nbr
		st_adr_nbr_sfx_to_cde
		ste_frm_nbr
		ste_to_nbr
		st_adr_frm_nbr
		st_adr_nbr_sfx_frm_cde
		mncplt_nme
		route_serv_typ_dsc_2
		route_serv_nbr_2
		di_area_nme
		di_typ_dsc
		di_qlfr_nme
		lock_box_bag_to_nbr
		lock_box_bag_frm_nbr
		route_serv_typ_dsc_4
		route_serv_nbr_4
		pstl_cde
		text_record_flag
		cntry_cde
	*/

	/// <summary>
	/// caAddress is the return data record response for /api/ca/search 
	/// </summary>
	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caAddress
	{
		[DataMember]
		public string id { get; set; }

		/// <summary>
		/// Record Type Code
		/// 
		/// Defines the type of address record. Valid values are:
		/// 1. Street Address Record
		/// 2. Street Served by Route Record
		/// 3. Lock Box Address Record
		/// 4. Route Service Address Record
		/// 5. General Delivery Address Record
		/// </summary>

		[DataMember]
		public int rec_typ_cde { get; set; }

		/// <summary>
		/// Address Type Code
		/// 
		/// A code denoting whether an address is in the form of a civic address or a delivery installation address. Valid values are:
		///	1. Civic (Street) Address Format
		///	2. Delivery Installation (Station) Address Format
		/// </summary>

		[DataMember]
		public int adr_typ_cde { get; set; }

		/// <summary>
		/// Province Code
		/// 
		/// Alphabetic code identifying the province geographically. Valid values are accessible through a query function.
		/// </summary>

		[DataMember]
		public string prov_cde { get; set; }

		/// <summary>
		/// Directory Area Name
		/// 
		/// Major community or greater municipality grouping that contains the street address or its delivery installation. Should not be used.
		/// </summary>

		[DataMember]
		public string drctry_area_nme { get; set; }

		/// <summary>
		/// Street Name
		/// 
		/// Official civic name of a roadway or artery
		/// </summary>

		[DataMember]
		public string st_nme { get; set; }

		/// <summary>
		/// Street Type Code
		/// 
		/// Official description used to identify the type of roadway or artery. Valid values are accessible through a query function.
		/// </summary>

		[DataMember]
		public string st_typ_cde { get; set; }

		/// <summary>
		/// Street Direction Code
		/// 
		/// Street direction component of an official street name. Valid values are accessible through a query function.
		/// </summary>

		[DataMember]
		public string st_drctn_cde { get; set; }

		/// <summary>
		/// Street Address Sequence Code
		/// 
		/// This code identifies the sequence associated with the range of street numbers. Valid values are: 
		/// 1. Odd 
		/// 2. Even 
		/// 3. Mixed
		/// </summary>

		[DataMember]
		public int? st_adr_seq_cde { get; set; }

		public string StAdrSeqCode()
		{
			string code = "";
			if (st_adr_seq_cde != null)
			{
				switch (st_adr_seq_cde) //static private dictionary?
				{
					case 1:
						code = "Odd"; break;
					case 2:
						code = "Even"; break;
					case 3:
						code = "Mixed"; break;
				}
			}
			return code;
		}


		/// <summary>
		/// Street Address to Number
		/// 
		/// The highest street number in a range of municipal street addresses.
		/// </summary>

		[DataMember]
		public int? st_adr_to_nbr { get; set; }

		/// <summary>
		/// Street Address Number Suffix to Code
		/// 
		/// The address suffix associated with the street address to number. Examples of street numbers with suffixes are 14 1/2 and 22B. A numeric number denotes a fractional suffix. Valid values are:
		/// 1. 1/4 
		/// 2. 1/2
		/// 3. 3/4
		/// </summary>

		[DataMember]
		public string st_adr_nbr_sfx_to_cde { get; set; }

		/// <summary>
		/// Suite to Number
		/// 
		/// Highest value in a range of suites or apartments.
		/// </summary>

		[DataMember]
		public string ste_frm_nbr { get; set; }

		/// <summary>
		/// Municipality Name
		///
		/// A municipality is any village, town or city in Canada that is recognized as a valid mailing address by Canada Post.
		/// </summary>

		[DataMember]
		public string ste_to_nbr { get; set; }


		/// <summary>
		/// Street Address from Number
		///
		/// The lowest street number in a range of municipal street addresses.
		/// </summary>

		[DataMember]
		public int? st_adr_frm_nbr { get; set; }

		/// <summary>
		/// Street Address Number Suffix from Code
		///
		/// The address suffix associated with the street address from number. Examples of street numbers with suffixes are 14 1/2 and 22B. A numeric number denotes a fractional suffix. Valid values are:
		/// 1. 1/4 
		/// 2. 1/2
		/// 3. 3/4
		/// </summary>

		[DataMember]
		public string st_adr_nbr_sfx_frm_cde { get; set; }


		/// <summary>
		/// Municipality Name
		/// 
		/// A municipality is any village, town or city in Canada that is recognized as a valid mailing address by Canada Post.
		/// </summary>

		[DataMember]
		public string mncplt_nme { get; set; }

		//not used public int? route_serv_box_to_nbr { get; set; }
		//not used public int? route_serv_box_frm_nbr { get; set; }

		/// <summary>
		/// Route Service Type Description for type 2 records
		///
		/// The code that identifies the type of route service. Valid values are: 
		/// - RR - Rural Route 
		/// - SS - Suburban Service 
		/// - MR - Mobile Route 
		/// - GD - General Delivery 
		/// </summary>

		[DataMember]
		public string route_serv_typ_dsc { get; set; }

		/// <summary>
		/// Route Service Number for type 2 records
		///
		/// Number that identifies Rural Route, Suburban Service or Mobile Route delivery mode
		/// </summary>

		[DataMember]
		public int? route_serv_nbr { get; set; }

		/// <summary>
		/// Delivery Installation Area Name
		///
		/// The name of a village, town, municipality or city that forms part of a Delivery Installation Name. While this field is populated, it is USUALLY not required to be displayed as part of the address.
		/// </summary>

		[DataMember]
		public string di_area_nme { get; set; }

		/// <summary>
		/// Delivery Installation Type Description
		///
		/// The category of delivery installation. Valid values are accessible through a query function.
		/// </summary>

		[DataMember]
		public string di_typ_dsc { get; set; }

		/// <summary>
		/// Delivery Installation Qualifier Name
		///
		/// When more than one delivery installation serves an area, the qualifier name uniquely identifies the delivery installation.
		/// </summary>

		[DataMember]
		public string di_qlfr_nme { get; set; }

		/// <summary>
		/// Lock Box bag to Number
		///
		/// Highest number in a range of lock boxes.
		/// </summary>

		[DataMember]
		public int? lock_box_bag_to_nbr { get; set; }

		/// <summary>
		/// Lock Box bag from Number
		///
		/// Lowest number in a range of lock boxes.
		/// </summary>

		[DataMember]
		public int? lock_box_bag_frm_nbr { get; set; }

		/*
		/// <summary>
		/// Route Service Type Description for type 4 records
		///
		/// The code that identifies the type of route service. Valid values are the same as for type 2 records.
		/// </summary>

		[DataMember]
		public string route_serv_typ_dsc_4 { get; set; }

		/// <summary>
		/// Route Service Number for type 4 records
		///
		/// Number that identifies Rural Route, Suburban Service or Mobile Route delivery mode
		/// </summary>

		[DataMember]
		public int? route_serv_nbr_4 { get; set; }
		*/

		/// <summary>
		/// Postal Code
		///
		/// A ten character, alpha numeric combination (ANANAN) assigned to one or more postal addresses. The postal code is an integral part of every postal address in Canada and is required for the mechanized processing of mail. Postal codes are also used to identify various CPC processing facilities and delivery installations.
		/// </summary>

		[DataMember]
		public string pstl_cde { get; set; }

		// <summary>
		// Delivery Installation Postal Code
		//
		// The postal code of the delivery installation responsible for delivery to the postal code. Not Used.
		// </summary>
		//not used public string di_pstl_cde { get; set; }

		/// <summary>
		/// Text Record Flag
		///
		/// Defines the type of record in the TEXT lookup table 
		/// * A = Building name record 
		/// * B = Large Volume Receiver Name (Street) record 
		/// * C = Government Name (Street) record 
		/// * D = Large Volume Receiver Name (PO BOX) record 
		/// * E = Government Name (PO BOX) record 
		/// * F = General Delivery record
		/// * R = Road Segment record
		/// * V = Virtual Road Segment record
		/// </summary>

		[DataMember]
		public string text_record_flag { get; set; }

		/// <summary>
		/// Country Code
		/// </summary>

		[DataMember]
		public string cntry_cde { get; set; }

		/// <summary>
		/// Testing
		/// </summary>
		[DataMember]
		public HashSet<string> cpc_nrn_segs { get; set; }

		/// <summary>
		/// Original StreetPerfect internal record when debugging
		/// </summary>	

		[DataMember]
		public string orig_rec { get; set; }
	}

	 
	[DataContract(Namespace =SPConst.DataNamespace)]
	public class usAddress
	{
		/// <summary>
		/// Record Type Code
		/// 
		/// This code identifies the record types. Valid values are: 
		/// * B = Building 
		/// * F = Firm name 
		/// * G = General Delivery 
		/// * H = Highrise 
		/// * M = Military 
		/// * P = PO Box 
		/// * R – Rural 
		/// * S – Urban 
		/// * U – Unique
		/// * \* – Generic
		/// </summary>

		[DataMember]
		public string RecordType { get; set; }

		[DataMember]
		public string CityName { get; set; }

		[DataMember]
		public string StateAbbreviation { get; set; }

		[DataMember]
		public string ZipCode { get; set; }

		[DataMember]
		public string PlusFourAddonLow { get; set; }

		[DataMember]
		public string PlusFourAddonHigh { get; set; }

		[DataMember]
		public string StreetNumberLow { get; set; }

		[DataMember]
		public string StreetNumberHigh { get; set; }

		[DataMember]
		public string StreetPreDirection { get; set; }

		[DataMember]
		public string StreetName { get; set; }

		[DataMember]
		public string StreetSuffix { get; set; }

		[DataMember]
		public string StreetPostDirection { get; set; }

		[DataMember]
		public string UnitType { get; set; }

		[DataMember]
		public string UnitNumberLow { get; set; }

		[DataMember]
		public string UnitNumberHigh { get; set; }

		[DataMember]
		public string PrivateMailBoxNumber { get; set; }

		[DataMember]
		public string LocationName { get; set; }

		/// <summary>
		/// Original StreetPerfect record when debugging
		/// </summary>	

		[DataMember]
		public string orig_rec { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class GetInfoResponse
	{

		[DataMember]
		public List<string> info { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}


	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caAddressRequest
	{
		[DataMember]
		public Options options { get; set; }

		[DataMember]
		public string recipient { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caCorrectionResponse
	{
		[DataMember]
		public string recipient { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }

		[DataMember]
		public string extra_information { get; set; }

		[DataMember]
		public string unidentified_component { get; set; }

		/// <summary>
		/// * V = Submitted address is Valid
		/// * C = Submitted address is Corrected
		/// * N = Submitted address is Not correct
		/// * F = Submitted address is Foreign
		/// </summary>

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public long msecs { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}


	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caParseResponse
	{

		[DataMember]
		public string address_type { get; set; }

		[DataMember]
		public string street_number { get; set; }

		[DataMember]
		public string street_suffix { get; set; }

		[DataMember]
		public string street_name { get; set; }

		[DataMember]
		public string street_type { get; set; }

		[DataMember]
		public string street_direction { get; set; }

		[DataMember]
		public string unit_type { get; set; }

		[DataMember]
		public string unit_number { get; set; }

		[DataMember]
		public string service_type { get; set; }

		[DataMember]
		public string service_number { get; set; }

		[DataMember]
		public string service_area_name { get; set; }

		[DataMember]
		public string service_area_type { get; set; }

		[DataMember]
		public string service_area_qualifier { get; set; }

		[DataMember]
		public string extra_information { get; set; }

		[DataMember]
		public string unidentified_component { get; set; }

		/// <summary>
		/// Valid Address
		/// * P = Parsed &amp; Valid
		/// 
		/// Invalid Address
		/// * I = Parsed &amp; Invalid
		/// </summary>

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caSearchResponse
	{

		[DataMember]
		public int response_count { get; set; }

		[DataMember]
		public long t_exec_ms { get; set; }

		[DataMember]
		public List<caAddress> response_address_list { get; set; }

		/// <summary>
		/// * N = At least one record found
		/// * X = No records found
		/// </summary>

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}




	// CA legacy non-ProcessAddress functions

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caFetchAddressRequest
	{
		[DataMember]
		public Options options { get; set; }

		[DataMember]
		public int? street_number { get; set; }

		[DataMember]
		public string unit_number { get; set; }

		[DataMember]
		public string postal_code { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caFetchAddressResponse
	{

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }
		//public string country { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caFormatAddressRequest
	{

		[DataMember]
		public Options options { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }
		//public string country { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caFormatAddressResponse
	{

		[DataMember]
		public string format_line_one { get; set; }

		[DataMember]
		public string format_line_two { get; set; }

		[DataMember]
		public string format_line_three { get; set; }

		[DataMember]
		public string format_line_four { get; set; }

		[DataMember]
		public string format_line_five { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}


	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caValidateAddressRequest
	{
		[DataMember]
		public Options options { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }
		//public string country { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caValidateAddressResponse
	{

		[DataMember]
		public List<string> function_messages { get; set; } = new List<string>();

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}


	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caQueryRequest
	{
		[DataMember]
		public Options options { get; set; }

		/// <summary>
		/// Query Option Value
		///  
		/// 11. Postal Code Search <br/>
		/// Input: Requires postal code <br/>
		/// Output: Returns pairs of records in the output array <br/>
		///     - 132-byte format range records
		/// 	- 232-byte CPC raw data range records
		/// 
		///   
		/// 12. Text Record Search <br/>
		/// Input: Requires postal code <br/>
		/// Output: Returns additional text information in the output array <br/>
		/// 
		/// 
		/// 13. Postal Code Search <br/>
		/// Input: Requires postal code <br/>
		/// Output: Returns records in the output array <br/>
		///     - 232-byte CPC raw data range records
		/// 
		/// 
		/// 14. CPC Raw Data Range Record Format <br/>
		/// Input: Requires CPC raw data range record in address line field <br/>
		/// Output: Returns two 63-byte format address records in output array  <br/>
		/// 
		/// 
		/// 16. Postal Code Search <br/>
		/// Input: City and province <br/>
		/// Output: Returns postal codes in the output array <br/>
		/// 
		/// 
		/// 20. Rural Address Search all types (GD/ PO BOX/RR) <br/>
		/// Input: City and province (optional) <br/>
		/// Output: Returns pairs of records in the output array
		///     - 132-byte format range records *
		///     - 232-byte CPC raw data range records *
		/// 
		/// 
		/// 21. Urban Address Search - ‘STREET’ types <br/>
		/// Input: Minimum requirement: street name and city. Additional components such as civic number, street type and province will yield more specific results. Use address line field for input. <br/>
		/// Output: Returns pairs of records in the output array
		///     - 132-byte format range records
		///     - 232-byte CPC raw data range records
		/// 	
		/// 	
		/// 23. Rural Address Search ‘PO BOX’ types <br/>
		/// Input: Minimum requirement: city. Additional components such as PO BOX number and province will yield more specific results. Use address line field for input. <br/>
		/// Output: Returns pairs of records in the output array
		///     - 132-byte format range records
		///     - 232-byte CPC raw data range records
		/// 
		/// 
		/// 24. Rural Address Search ‘RR/SS/MR’ types <br/>
		/// Input: Minimum requirement: city. Additional components such as route service number and province will yield more specific results. Use address line field for input. <br/>
		/// Output: Returns pairs of records in the output array
		///     - 132-byte format range records
		///     - 232-byte CPC raw data range records
		/// 	
		/// 	
		/// 25. Rural Address Search ‘GD’ types <br/>
		/// Input: Minimum requirement: city. Additional components such as province will yield more specific results. Use address line field for input. <br/>
		/// Output: Returns pairs of records in the output array
		///     - 132-byte format range records
		///     - 232-byte CPC raw data range records
		/// 	
		/// 	
		/// 26. CPC Raw Data Range Search <br/>
		/// Input: Requires CPC raw data range record. Use address line field for input. <br/>
		/// Output: Returns all address matches in CPC raw data range record format in the output array. <br/>
		/// 
		/// 
		/// 31. Return street type table entries.
		/// 
		/// 
		/// 32. Return street direction table entries.
		/// 
		/// 
		/// 33. Return route service type description table entries.
		/// 
		/// 
		/// 34. Return province code table entries.
		/// 
		/// 
		/// 35. Return service type table entries.
		/// 
		/// 
		/// 36. Return delivery installation type table entries.
		/// 
		/// 
		/// 37. Return country code table entries.
		/// 
		/// 
		/// 38. Return US state code table entries.
		/// 
		/// 
		/// 39. Return unit designator table entries.
		/// 
		/// 
		/// 310. Street name search <br/>
		/// Input: Partial street name. Use street name field for input. <br/>
		/// Output: Matching street name table records in the output array <br/>
		/// 
		/// 
		/// 311. Urban municipality name search <br/>
		/// Input: Partial municipality name. Use municipality name field for input. <br/>
		/// Output: Matching municipality name table records in the output array. <br/>
		/// 
		/// 
		/// 312. Rural municipality name search <br/>
		/// Input: Partial rural municipality name. Use municipality name field for input. <br/>
		/// Output: Matching municipality name table records in the output array. <br/>
		/// 
		/// 
		/// 313. Urban and Rural municipality name search <br/>
		/// Input: Partial municipality name. Use municipality name field for input. <br/>
		/// Output: Matching municipality name table records in the output array. <br/>
		/// 
		/// 
		/// 314. Municipality abbreviations search <br/>
		/// Input: Official municipality name and province. <br/>
		/// Output: Municipality abbreviations. <br/>
		/// 
		/// 
		/// 315. Return urban extra information table entries
		/// 
		/// 
		/// 316. Return rural extra information table entries
		/// 
		/// 
		/// 42. Text file search <br/>
		/// Input: Text string in address line field. <br/>
		/// Output: Postal Code and CPC Text information (company name, building name, etc). <br/>
		/// 
		/// 
		/// 43. Municipalities within province <br/>
		/// Input: Municipality name (may be partial) and province. <br/>
		/// Output: All municipalities within province starting with input municipality name string. This may be effective for implementing a "drill down" feature. <br/>
		/// 
		/// 
		/// 44. Street names within city and province <br/>
		/// Input: Street name (may be partial) in address line field and municipality and province. <br/>
		/// Output: All streets in input municipality and province starting with input street name string. This may be effective for implementing a "drill down" feature. <br/>
		/// 
		/// 
		/// Wildcard Searches <br/>
		/// Input any combination of: <br/>
		/// SP_IN_ST_NME, <br/>
		/// SP_IN_CITY <br/>
		/// SP_IN_PROV <br/>
		/// The civic number is optional in all cases. These searches allow partial entry of street name and city providing a wildcard capability matching everything which starts with the input string. Large volumes of data may be returned from these searches so it is important to wait until a few characters have been entered before executing the function. Additional information is particularly effective at reducing the size of the result set, especially a civic number and the first character of the municipality and / or province. <br/>
		/// QT = 6 Returns a 132-byte format range record for each row in the CPC db matching the input suitable for display. <br/>
		/// QT = 7 Returns pairs of records <br/>
		/// - 132-byte format range records (as above) 
		/// - 232-byte CPC raw data range records
		/// QO controls the sort order of the output data <br/>
		/// 
		/// 
		/// 61. (71) Province : Municipality : Wildcard Street Name
		/// 
		/// 
		/// 62. (72) Wildcard Street Name: Municipality : Province
		/// 
		/// 
		/// 63. (73) Province : Wildcard Street Name: Municipality
		/// 
		/// 
		/// 64. (74) Municipality : Wildcard Street Name: Province
		/// 
		/// 
		/// 65. (75) Municipality : Province : Wildcard Street Name
		/// 
		/// 
		/// 66. (76) Wildcard Street Name: Province : Municipality
		/// 
		/// 
		/// 67. (77) Province : Municipality : Simple Street Name
		/// 
		/// 
		/// 68. (78) Simple Street Name : Municipality : Province
		/// 
		/// 
		/// 69. (79) Simple Street Name (No civic number)
		/// 
		/// </summary>
		 
		[DataMember]
		public int query_option { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }

		/// <summary>
		/// max records to return, 100 is default
		/// </summary>
		[DataMember]
		public int? max_returned { get; set; } = 100;
		//public string country { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caQueryResponse
	{

		[DataMember]
		public List<string> function_messages { get; set; }


		[DataMember]
		public List<caAddress> address_list { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}




	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caQueryWildcardRequest
	{
		[DataMember]
		public Options options { get; set; }

		/// <summary>
		/// 
		/// sort_by controls the sort order of the output data <br/>
		/// 
		/// 1. Province : Municipality : Wildcard Street Name
		/// 2. Wildcard Street Name: Municipality : Province
		/// 3. Province : Wildcard Street Name: Municipality
		/// 4. Municipality : Wildcard Street Name: Province
		/// 5. Municipality : Province : Wildcard Street Name
		/// 6. Wildcard Street Name: Province : Municipality
		/// 7. Province : Municipality : Simple Street Name
		/// 8. Simple Street Name : Municipality : Province
		/// 9. Simple Street Name (No civic number)
		/// 
		/// </summary>

		[DataMember]
		public int sort_by { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }
		/// <summary>
		/// The maximum number of results to return, actual maximum is 1000. Null or zero returns default max of 1000.
		/// </summary>

		[DataMember]
		public int? max_returned { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caQueryWildcardResponse
	{

		[DataMember]
		public long t_exec_ms { get; set; }

		[DataMember]
		public string addr_num { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public int response_count { get; set; }

		[DataMember]
		public List<caAddress> address_list { get; set; }
	}


	// 133 byte SP 'view' record returned by query level 6 & 7 functions -- NOT used
	[DataContract(Namespace =SPConst.DataNamespace)]
	public class caRangeAddress
	{
		/// <summary>
		/// Street Address Sequence Code
		/// 
		/// This code identifies the sequence associated with the range of street numbers. Valid values are: 
		/// 1. Odd 
		/// 2. Even 
		/// 3. Consecutive
		/// </summary>

		[DataMember]
		public string st_adr_seq_cde { get; set; }

		/// <summary>
		/// Street Address to Number
		/// 
		/// The highest street number in a range of municipal street addresses.
		/// </summary>

		[DataMember]
		public int? st_adr_frm_nbr { get; set; }

		/// <summary>
		/// Street Address from Number
		///
		/// The lowest street number in a range of municipal street addresses.
		/// </summary>

		[DataMember]
		public int? st_adr_to_nbr { get; set; }

		/// <summary>
		/// Street Name
		/// 
		/// Official civic name of a roadway or artery
		/// </summary>

		[DataMember]
		public string st_nme { get; set; }

		/// <summary>
		/// Route Service Type Description for type 2 records
		///
		/// The code that identifies the type of route service. Valid values are: 
		/// - RR - Rural Route 
		/// - SS - Suburban Service 
		/// - MR - Mobile Route 
		/// - GD - General Delivery 
		/// </summary>

		[DataMember]
		public string route_serv_typ_dsc_2 { get; set; }

		/// <summary>
		/// Route Service Number for type 2 records
		///
		/// Number that identifies Rural Route, Suburban Service or Mobile Route delivery mode
		/// </summary>

		[DataMember]
		public int? route_serv_nbr_2 { get; set; }

		/// <summary>
		/// Municipality Name
		///
		/// A municipality is any village, town or city in Canada that is recognized as a valid mailing address by Canada Post.
		/// </summary>

		[DataMember]
		public string mncplt_nme { get; set; }

		/// <summary>
		/// Province Code
		/// 
		/// Alphabetic code identifying the province geographically. Valid values are accessible through a query function.
		/// </summary>

		[DataMember]
		public string prov_cde { get; set; }

		/// <summary>
		/// Postal Code
		///
		/// A ten character, alpha numeric combination (ANANAN) assigned to one or more postal addresses. The postal code is an integral part of every postal address in Canada and is required for the mechanized processing of mail. Postal codes are also used to identify various CPC processing facilities and delivery installations.
		/// </summary>

		[DataMember]
		public string pstl_cde { get; set; }

		/// <summary>
		/// Original StreetPerfect internal record when debugging
		/// </summary>	

		[DataMember]
		public string orig_rec { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class usAddressRequest
	{
		[DataMember]
		public Options options { get; set; } = null;

		[DataMember]
		public string firm_name { get; set; }

		[DataMember]
		public string urbanization_name { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string state { get; set; }

		[DataMember]
		public string zip_code { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class usCorrectionResponse
	{

		[DataMember]
		public string firm_name { get; set; }

		[DataMember]
		public string urbanization_name { get; set; }

		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string state { get; set; }

		[DataMember]
		public string zip_code { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}


	[DataContract(Namespace =SPConst.DataNamespace)]
	public class usParseResponse
	{

		[DataMember]
		public string address_type { get; set; }

		[DataMember]
		public string street_number { get; set; }

		[DataMember]
		public string street_pre_direction { get; set; }

		[DataMember]
		public string street_name { get; set; }

		[DataMember]
		public string street_type { get; set; }

		[DataMember]
		public string street_post_direction { get; set; }

		[DataMember]
		public string secondary_type { get; set; }

		[DataMember]
		public string secondary_number { get; set; }

		[DataMember]
		public string service_type { get; set; }

		[DataMember]
		public string service_number { get; set; }

		[DataMember]
		public string delivery_point_barcode { get; set; }

		[DataMember]
		public string congressional_district { get; set; }

		[DataMember]
		public string county_name { get; set; }

		[DataMember]
		public string county_code { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}

	[DataContract(Namespace =SPConst.DataNamespace)]
	public class usSearchResponse
	{

		[DataMember]
		public int response_count { get; set; }

		[DataMember]
		public List<usAddress> response_address_list { get; set; }

		/// <summary>
		/// Valid
		/// * S = Single Response
		/// * D = Default Response
		/// 
		/// Invalid or Multiple
		/// * I = Invalid
		/// * M = Multiple Response
		/// </summary>

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}


	// not used
	[DataContract(Namespace =SPConst.DataNamespace)]
	public class usDeliveryInformationResponse
	{

		[DataMember]
		public string city_abbreviation { get; set; }

		[DataMember]
		public string post_office_city { get; set; }

		[DataMember]
		public string post_office_state { get; set; }

		[DataMember]
		public string delivery_point_bar_code { get; set; }

		[DataMember]
		public string carrier_route { get; set; }

		[DataMember]
		public string auto_zone_indicator { get; set; }

		[DataMember]
		public string lot_number { get; set; }

		[DataMember]
		public string lot_code { get; set; }

		[DataMember]
		public string lacs_code { get; set; }

		[DataMember]
		public string county_code { get; set; }

		[DataMember]
		public string finance_number { get; set; }

		[DataMember]
		public string congressional_district { get; set; }

		[DataMember]
		public string pmb_designator { get; set; }

		[DataMember]
		public string pmb_number { get; set; }


		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }

		[DataMember]
		public List<string> function_messages { get; set; }
	}

}

