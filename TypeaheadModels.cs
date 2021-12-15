using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StreetPerfect.Models
{
	/// <summary>
	/// I added these here for the REST Client to access
	/// they are NOT used by the Native client (use the legacy (query) wildcard search instead)
	/// </summary>

	[DataContract(Namespace = SPConst.DataNamespace)]
	public class caTypeaheadRequest
	{
		/// <summary>
		/// The user entered address search line
		/// </summary>
		[DataMember]
		public string address_line { get; set; }

		/// <summary>
		/// Allows the query to be restricted to a specific city.  City searches are prefixed (like a trailing wildcard; miss*)
		/// and dashes are ignored so users can use them or not.
		/// </summary>
		[DataMember]
		public string city { get; set; }

		/// <summary>
		/// Allows the query to be restricted to a province. Use the 2 letter province notation.
		/// </summary>
		[DataMember]
		public string province { get; set; }


		/// <summary>
		/// Allows the query to be restricted to a postal code. (Will also be treated with a trailing wildcard; l5n*)
		/// </summary>
		[DataMember]
		public string postal_code { get; set; }

		/// <summary>
		/// Tokenizes the users address_line causing each token to be (prefixed) searched through the full address.
		/// </summary>
		[DataMember]
		public bool tokenize_qry { get; set; }


		/// <summary>
		/// The maximum number of results to return, actual maximum is 200. Null or zero returns default max of 20.
		/// </summary>

		[DataMember]
		public int? max_returned { get; set; }


		/// <summary>
		/// The record to start returning records at. Used for paging large result sets. Record numbering starts at zero.
		/// </summary>
		[DataMember]
		public int? start_rec { get; set; }

	}

	[DataContract(Namespace = SPConst.DataNamespace)]
	public class caTypeaheadAddr
	{
		/// <summary>
		/// The Id of the full address record allowing you to fetch later
		/// </summary>
		[DataMember]
		public string id { get; set; }

		/// <summary>
		/// The formated address line for display
		/// </summary>
		[DataMember]
		public string addr { get; set; }
	}

	[DataContract(Namespace = SPConst.DataNamespace)]
	public class caTypeaheadResponse
	{
		/// <summary>
		/// The parsed street number from the users address line
		/// </summary>
		[DataMember]
		public int addr_num { get; set; }

		/// <summary>
		/// The parsed unit number from the users address line (must be before street number with a dash, nnn-ssss st name)
		/// </summary>
		[DataMember]
		public string unit_num { get; set; }

		/// <summary>
		/// The parsed street suffix from the users address line
		/// </summary>
		[DataMember]
		public string suffix { get; set; }

		/// <summary>
		/// Number of returned addresses
		/// </summary>
		[DataMember]
		public int count { get; set; }

		/// <summary>
		/// Starting address record. (passed in caTypeaheadRequest)
		/// </summary>
		[DataMember]
		public int start_rec { get; set; }		
		
		/// <summary>
		/// The total number of records that match the search. This can be greater than count.
		/// </summary>
		[DataMember]
		public int total_hits { get; set; }


		/// <summary>
		/// The returned addresses, each has an Id and a display string. Use the FetchById API to get the full address on user selection.
		/// 
		/// Will be null/missing for typeaheadRec
		/// </summary>
		[DataMember]
		public List<caTypeaheadAddr> address_lines { get; set; }


		/// <summary>
		/// The returned addresses, each has an Id and a display string. Use the FetchById API to get the full address on user selection.
		/// 
		/// Will be null/missing for normal typeahead
		/// </summary>
		[DataMember]
		public List<caAddress> recs { get; set; }


		/// <summary>
		/// Server-side execution time in milliseconds
		/// </summary>
		[DataMember]
		public long t_exec_ms { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}


	[DataContract(Namespace = SPConst.DataNamespace)]
	public class caTypeaheadFetchRequest
	{
		[DataMember]
		public Options options { get; set; }

		/// <summary>
		/// Fetch addr using the id (returned by typeahead)
		/// </summary>
		[DataMember]
		[Required]
		public string id { get; set; }

		/// <summary>
		/// Enable autocorrect/optimize to call ca/correction on your behalf.
		/// 
		/// Note that the passed options are used when making this call.
		/// </summary>
		[DataMember]
		public bool? autocorrect { get; set; }


		/// <summary>
		/// Return all discrete components of the returned addresses.
		/// 
		/// (This includes any modified components from an auto-correction request.)
		/// </summary>
		[DataMember]
		public bool? return_components { get; set; }

		/// <summary>
		/// Enable autoformat to call ca/format on your behalf.
		/// 
		/// Note that the options are used when making this call.
		/// </summary>
		//[DataMember]
		//public bool? autoformat { get; set; }

		[DataMember]
		public int? street_num { get; set; }

		[DataMember]
		public string street_suffix { get; set; }

		[DataMember]
		public string unit_num { get; set; }

		[DataMember]
		public string postal_code { get; set; }


	}


	[DataContract(Namespace = SPConst.DataNamespace)]
	public class caTypeaheadFetchResponse
	{
		[DataMember]
		public string address_line { get; set; }

		[DataMember]
		public string city { get; set; }

		[DataMember]
		public string province { get; set; }

		[DataMember]
		public string postal_code { get; set; }

		/// <summary>
		/// used if autocorrect is enabled
		/// </summary>
		[DataMember]
		public string unidentified_component { get; set; }

		/// <summary>
		/// used if autocorrect is enabled
		/// </summary>
		[DataMember]
		public List<string> function_messages { get; set; }

		/// <summary>
		/// Data record representing this address
		/// </summary>
		[DataMember]
		public caAddress addr_rec { get; set; }

		/// <summary>
		/// if return_components == true (in the fetch request) then discrete address components will be returned in this object
		/// </summary>
		[DataMember]
		public caParseResponse components { get; set; }

		[DataMember]
		public string status_flag { get; set; }

		[DataMember]
		public string status_messages { get; set; }
	}


}
