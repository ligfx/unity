using System;

using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class DeleteChannelGroupRequestBuilder: PubNubNonSubBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>, IPubNubNonSubscribeBuilder<DeleteChannelGroupRequestBuilder, PNChannelGroupsDeleteGroupResult>
    {      
        public DeleteChannelGroupRequestBuilder(PubNubUnity pn):base(pn){

        }
        private string ChannelGroupToDelete { get; set;}

        public void ChannelGroup(string channelGroup){
            ChannelGroupToDelete = channelGroup;
            ChannelGroupsToUse = new List<string>(){ChannelGroupToDelete};
        }
        
        #region IPubNubBuilder implementation

        public void Async(Action<PNChannelGroupsDeleteGroupResult, PNStatus> callback)
        {
            this.Callback = callback;
            Debug.Log ("DeleteChannelGroupRequestBuilder Async");

            if (string.IsNullOrEmpty (ChannelGroupToDelete)) {
                Debug.Log("ChannelGroup to delete to empty");
                return;
            }
            base.Async(callback, PNOperationType.PNRemoveGroupOperation, PNCurrentRequestType.NonSubscribe, this);
        }
        #endregion

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.RespType = PNOperationType.PNRemoveGroupOperation;

            /* Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                null, 
                "", 
                ChannelGroupToDelete,
                this.PubNubInstance.PNConfig.UUID,
                this.PubNubInstance.PNConfig.Secure,
                this.PubNubInstance.PNConfig.Origin,
                this.PubNubInstance.PNConfig.AuthKey,
                this.PubNubInstance.PNConfig.SubscribeKey,
                this.PubNubInstance.Version
            ); */
            Uri request = BuildRequests.BuildRemoveChannelsFromChannelGroupRequest(
                null, 
                "", 
                ChannelGroupToDelete,
                ref this.PubNubInstance
            );
            this.PubNubInstance.PNLog.WriteToLog(string.Format("RunPNChannelGroupsAddChannel {0}", request.OriginalString), PNLoggingMethod.LevelInfo);
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        // protected override void CreateErrorResponse(Exception exception, bool showInCallback, bool level){
            
        // }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            PNChannelGroupsDeleteGroupResult pnChannelGroupsDeleteGroupResult = new PNChannelGroupsDeleteGroupResult();
            Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
            PNStatus pnStatus = new PNStatus();
            if (dictionary!=null && dictionary.ContainsKey("error") && dictionary["error"].Equals(true)){
                pnChannelGroupsDeleteGroupResult = null;
                pnStatus.Error = true;
                //TODO create error data
            } else if(dictionary!=null) {
                object objMessage;
                dictionary.TryGetValue("message", out objMessage);
                if(objMessage!=null){
                    pnChannelGroupsDeleteGroupResult.Message = objMessage.ToString();
                }
            } else {
                pnChannelGroupsDeleteGroupResult = null;
                pnStatus.Error = true;
            }
            Callback(pnChannelGroupsDeleteGroupResult, pnStatus);
        }
        
    }
}
