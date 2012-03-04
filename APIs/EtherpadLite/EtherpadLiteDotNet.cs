/// <license>
///2011 Jonathon Smith
///
///Licensed under the Apache License, Version 2.0 (the "License");
///you may not use this file except in compliance with the License.
///You may obtain a copy of the License at
///
///   http://www.apache.org/licenses/LICENSE-2.0
///
///Unless required by applicable law or agreed to in writing, software
///distributed under the License is distributed on an "AS-IS" BASIS,
///WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///See the License for the specific language governing permissions and
///limitations under the License.
/// </license>

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web.Script.Serialization;
using O2.Kernel.ExtensionMethods;
//O2Ref:System.Web.Extensions.dll

namespace Etherpad
{
    public enum EtherpadReturnCodeEnum
    {
        Ok,
        InvalidParameters,
        InternalError,
        InvalidFunction,
        InvalidAPIKey
    }

    public class EtherpadLiteDotNet
    {
        private const string APIVersion = "1";
        private string ApiKey { get; set; }
        private UriBuilder BaseURI { get; set; }

		public EtherpadLiteDotNet(string apiKey, string host) : this(apiKey, host, 0)
		{
		}
        public EtherpadLiteDotNet(string apiKey, string host, int port)//, int port = 0)
        {
            ApiKey = apiKey;
            if (port == 0)
            {
                BaseURI = new UriBuilder("http", host);
            }
            else
            {
                BaseURI = new UriBuilder("http", host, port);
            }

        }

		private T CallAPI<T>(string functionName)
			where T : EtherpadResponse
		{
			return CallAPI<T>(functionName, null);
		}
		
		/*private T CallAPI<T>(string functionName, string[,] query )
		{
			return CallAPI<T>(functionName,query, null);
		}*/
		
        private T CallAPI<T>(string functionName,string[,] query) // ,  Type customReturnType)  //string[,] query = null, Type customReturnType = null )
        	where T : EtherpadResponse
        {
            BaseURI.Path = "api/" + APIVersion + "/" + functionName;
            BaseURI.Query = BuildQueryString(query);
			
            #region Get Response And Deserialize it
            T responseObject;
            using (var response = (HttpWebResponse)WebRequest.Create(BaseURI.Uri).GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var responseText = reader.ReadToEnd();
                //if (customReturnType == null)
                {
                    responseObject = js.Deserialize<T>(responseText);
                }
                //else
                //{
                //    responseObject = (EtherpadResponse)js.Deserialize(responseText,customReturnType);
                //}
                responseObject.JSON = responseText;
            }
            #endregion

            #region Check for Errors In Reponse
            switch (responseObject.Code)
            {
                case EtherpadReturnCodeEnum.InternalError:
                	"An error has occured in Etherpad: ".error(responseObject.Message);
                	break;
                    //throw new SystemException("An error has occured in Etherpad: " + responseObject.Message);
                case EtherpadReturnCodeEnum.InvalidAPIKey:
                	"The API key supplied is invalid.".error();
                	break;
                    //throw new ArgumentException("The API key supplied is invalid.");
                case EtherpadReturnCodeEnum.InvalidFunction:
                	"The function name passed is invalid.".error(responseObject.Message);
                	break;
                    //throw new MissingMethodException("The function name passed is invalid.", responseObject.Message);
                case EtherpadReturnCodeEnum.InvalidParameters:
                	"An invalid parameter has been passed to the function.".error(responseObject.Message);
                	break;
                    //throw new ArgumentException("An invalid parameter has been passed to the function.", responseObject.Message);
                 
            }
            #endregion

            return responseObject;
        }

        private string BuildQueryString(string[,] query)
        {
            //By calling ParseQueryString with an empty string you get an empty HttpValueCollection which cannot be created any other way as it is a private class
            var queryCollection = System.Web.HttpUtility.ParseQueryString(String.Empty);
            queryCollection.Add("apikey", ApiKey);

            if (query != null)
            {
                int queryLength = query.GetLength(0) - 1;
                for (int i = 0; i <= queryLength; i++)
                {
                    queryCollection.Add(query[i, 0], query[i, 1]);
                }
            }
        
            return queryCollection.ToString();
        }

        #region Groups

        public EtherpadResponseGroupID CreateGroup()
        {
            return (EtherpadResponseGroupID)CallAPI<EtherpadResponseGroupID>("createGroup", null);
        }

        public EtherpadResponseGroupID CreateGroupIfNotExistsFor(string groupMapper)
        {
            return CallAPI<EtherpadResponseGroupID>("createGroupIfNotExistsFor",
                new string[,] { { "groupMapper", groupMapper } });
        }

        public EtherpadResponse DeleteGroup(string groupID)
        {
            return CallAPI<EtherpadResponse>("deleteGroup",
                new string[,] { { "groupID", groupID } });
        }

        public EtherpadResponsePadIDs ListPads(string groupID)
        {
            return CallAPI<EtherpadResponsePadIDs>("listPads",
                new string[,] { { "groupID", groupID } });
        }

        public EtherpadResponsePadID CreateGroupPad(string groupID, string padName)
        {
            return CallAPI<EtherpadResponsePadID>("createGroupPad",
                new string[,] { { "groupID", groupID }, { "padName", padName } });
        }

        public EtherpadResponsePadID CreateGroupPad(string groupID, string padName, string text)
        {
            return CallAPI<EtherpadResponsePadID>("createGroupPad",
                new string[,] { { "groupID", groupID }, { "padName", padName }, { "text", text } });
        }

        #endregion

        #region Author

        public EtherpadResponseAuthorID CreateAuthor()
        {
            return CallAPI<EtherpadResponseAuthorID>("createAuthor", null);
        }

        public EtherpadResponseAuthorID CreateAuthor(string name)
        {
            return CallAPI<EtherpadResponseAuthorID>("createAuthor",
                new string[,] { { "name", name } });
        }

        public EtherpadResponseAuthorID CreateAuthorIfNotExistsFor(string authorMapper)
        {
            return CallAPI<EtherpadResponseAuthorID>("createAuthorIfNotExistsFor",
                new string[,] { { "authorMapper", authorMapper } });
        }

        public EtherpadResponseAuthorID CreateAuthorIfNotExistsFor(string authorMapper, string name)
        {
            return CallAPI<EtherpadResponseAuthorID>("createAuthorIfNotExistsFor",
                new string[,] { { "authorMapper", authorMapper }, { "name", name } });
        }

        #endregion

        #region Session

        public EtherpadResponseSessionID CreateSession(string groupID, string authorID, string validUntil)
        {
            return CallAPI<EtherpadResponseSessionID>("createSession",
                new string[,] { { "groupID", groupID }, { "authorID", authorID }, { "validUntil", validUntil } });
        }

        public EtherpadResponse DeleteSession(string sessionID)
        {
            return CallAPI<EtherpadResponse>("deleteSession",
                new string[,] { { "sessionID", sessionID } });
        }

        public EtherpadResponse GetSessionInfo(string sessionID)
        {
            return CallAPI<EtherpadResponse>("getSessionInfo",
                new string[,] { { "sessionID", sessionID } });
        }

        public EtherpadResponseSessionInfos ListSessionsOfGroup(string groupID) 
        {
            return CallAPI<EtherpadResponseSessionInfos>("listSessionsOfGroup",
                new string[,] { { "groupID", groupID } });
        }

        public EtherpadResponseSessionInfos ListSessionsOfAuthor(string authorID)
        {
            return CallAPI<EtherpadResponseSessionInfos>("listSessionsOfAuthor",
                new string[,] { { "authorID", authorID } });
        }

        #endregion

        #region Pad

        public EtherpadResponsePadText GetText(string padID)
        {
            return CallAPI<EtherpadResponsePadText>("getText",
                new string[,] { { "padID", padID } });
        }

        public EtherpadResponsePadText GetText(string padID, int rev)
        {
            return CallAPI<EtherpadResponsePadText>("getText",
                new string[,] { { "padID", padID }, { "rev", rev.ToString() } });
        }

        public EtherpadResponse SetText(string padID, string text)
        {
            return CallAPI<EtherpadResponse>("setText",
                new string[,] { { "padID", padID }, { "text", text } });
        }

        public EtherpadResponse CreatePad(string padID)
        {
            return CallAPI<EtherpadResponse>("createPad", 
                new string[,] {{"padID", padID}} );
        }

        public EtherpadResponse CreatePad(string padID, string text)
        {
            return CallAPI<EtherpadResponse>("createPad", 
                new string[,] { { "padID", padID } , { "text", text } });
        }

        public EtherpadResponsePadRevisions GetRevisionsCount(string padID)
        {
            return CallAPI<EtherpadResponsePadRevisions>("getRevisionsCount", 
                new string[,] { { "padID", padID } });
        }

        public EtherpadResponse DeletePad(string padID)
        {
            return CallAPI<EtherpadResponse>("deletePad", 
                new string[,] { { "padID", padID } });
        }

        public EtherpadResponsePadReadOnlyID GetReadOnlyID(string padID)
        {
            return CallAPI<EtherpadResponsePadReadOnlyID>("getReadOnlyID",
                   new string[,] { { "padID", padID } });
        }

        public EtherpadResponse SetPublicStatus(string padID, bool publicStatus)
        {
            return CallAPI<EtherpadResponse>("setPublicStatus",
                    new string[,] { { "padID", padID }, { "publicStatus", publicStatus.ToString() } });
        }

        public EtherpadResponsePadPublicStatus GetPublicStatus(string padID)
        {
            return CallAPI<EtherpadResponsePadPublicStatus>("getPublicStatus",
                   new string[,] { { "padID", padID } });
        }

        public EtherpadResponse SetPassword(string padID, string password)
        {
            return CallAPI<EtherpadResponse>("setPassword",
                   new string[,] { { "padID", padID } , { "password", password } });
        }

        public EtherpadResponsePadPasswordProtection IsPasswordProtected(string padID)
        {
            return CallAPI<EtherpadResponsePadPasswordProtection>("isPasswordProtected",
                   new string[,] { { "padID", padID } });
        }

        #endregion Pad

    }

    /// <summary>
    ///This class is returned by all the API calls
    ///If you wanted to reduce the number of classes needed
    ///The strong typing could be replaced by: public Dictionary<string,string> Data { get; set; } in the base class
    /// </summary>
    public class EtherpadResponse
    {
        public EtherpadReturnCodeEnum Code { get; set; } 
        public string Message { get; set; }        
        public string JSON { get; set; }
    }

    #region Classes to Strong Type Response

    public class EtherpadResponsePadID : EtherpadResponse
    {
        public DataPadID Data { get; set; }
    }

    public class DataPadID
    {
        public string PadID { get; set; }
    }


    public class EtherpadResponseGroupID : EtherpadResponse
    {
        public DataGroupID Data {get; set;}
    }

    public class DataGroupID
    {
        public string GroupID { get; set; } 
    }

    public class EtherpadResponsePadIDs : EtherpadResponse
    {
        public DataPadIDs Data { get; set; }
    }

    public class DataPadIDs
    {
        //public List<string> PadIDs { get; set; }
        public Dictionary<string,int> PadIDs { get; set; }
    }

    public class EtherpadResponseAuthorID : EtherpadResponse
    {
        public DataAuthorID Data { get; set; }
    }

    public class DataAuthorID
    {
        public string AuthorID { get; set; }
    }

    public class EtherpadResponseSessionID : EtherpadResponse
    {
        public DataSessionID Data { get; set; }
    }

    public class DataSessionID
    {
        public string SessionID { get; set; }
    }

    public class EtherpadResponseSessionInfos : EtherpadResponse
    {
        public IEnumerable<DataSessionInfo> Data { get; set; }
    }

    public class DataSessionInfo
    {
        public string GrouopID { get; set; }
        public string AuthorID { get; set; }
        public int ValidUntil { get; set; }
    }

    public class EtherpadResponsePadText : EtherpadResponse
    {
        public DataPadText Data { get; set; }
    }

    public class DataPadText
    {
        public string Text { get; set; }
    }

    public class EtherpadResponsePadRevisions : EtherpadResponse
    {
        public DataPadRevisions Data { get; set; }
    }

    public class DataPadRevisions
    {
        public int Revisions { get; set; }
    }

    public class EtherpadResponsePadReadOnlyID : EtherpadResponse
    {
        public DataPadReadOnlyID Data { get; set; }
    }

    public class DataPadReadOnlyID
    {
        public string ReadOnlyID { get; set; }
    }

    public class EtherpadResponsePadPublicStatus : EtherpadResponse
    {
        public DataPadPublicStatus Data { get; set; }
    }

    public class DataPadPublicStatus
    {
        public bool PublicStatus { get; set; }
    }

    public class EtherpadResponsePadPasswordProtection : EtherpadResponse
    {
        public DataPadPasswordProtection Data { get; set; }
    }

    public class DataPadPasswordProtection
    {
        public bool PasswordProtection { get; set; }
    }

    #endregion

}

