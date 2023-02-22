var con_Xrm;
(function(con_Xrm) {
    var ribbonUtility = (function() {
        function ribbonUtility() {}
        
        /**
         * @author: ABU
         * @description Call a getJiraTickets custom api.
         */
        ribbonUtility.testingJiraRibbon = async function(primaryControl){
            let formContext = primaryControl;
            let opportunityId = formContext.data.entity.getId().replace(/{|}/g, '');
            let request = {
                con_opportunityid: { guid: opportunityId},
                
                getMetadata:() => ({
                boundParameter: null,
                parameterTypes:{
                    con_opportunityid: {
                    typeName: 'Edm.Guid',
                    structuralProperty: 1  // Primitive Type
                    }
                }, 
                operationType: 0,
                operationName: 'con_GetJiraTickets'    
            })
            };

            Xrm.WebApi.online.execute(request).then(successCallback, errorCallback);
        }   
    
         async function successCallback (response){
            let res = await response;
            let json = await res.json();
            response = JSON.parse(json.con_response);
            console.log(response);
        }

        function errorCallback(error){
            console.log(error);
        }
        return ribbonUtility;
    
    })();
    con_Xrm.ribbonUtility = ribbonUtility;
})(con_Xrm || (con_Xrm = {}));