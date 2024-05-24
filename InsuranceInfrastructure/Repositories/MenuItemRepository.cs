using InsuranceCore.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceInfrastructure.Repositories
{
    public class MenuItemRepository
    {
        private static IList<MenuItems> _menus;

        public static IList<MenuItems> Menus()
        {
            if (_menus != null)
                return _menus;

            _menus = new List<MenuItems>
            {
                //MenuItems("Code","MenuName","MenuTitle","Controllers","Action","Parameter")
                //new MenuItems("A01","Admin","Admin", "Users","Index","01",1),
                //new MenuItems("A02","Roles","Admin", "Roles","RolesList","01",2),
                new MenuItems("A03","List Of Request","Insurance", "Insurance","GetAllInsurance",null,8),
                //new MenuItems("A04","Users","External", "Users","Index","01",1),
                new MenuItems("A05","Authorize Request","Insurance", "Insurance"," AuthorizeRequest",null,2),
                new MenuItems("A06","Assign Underwriter","Insurance", "Insurance","AssignUnderwriter",null,3),
                new MenuItems("A07","Review Certificate","Insurance", "Insurance","Review",null,5),
                new MenuItems("A08","Upload Certificate","Insurance", "Insurance","InsuranceCertificate",null,4),
                new MenuItems("A09","Assign ContractID","Insurance", "Insurance","AssignContractID",null,6),
                new MenuItems("A010","GetInsurance","Insurance", "Insurance","GetInsurance",null,7),
                 new MenuItems("A011","List Of Underwriter","Underwriters", "Underwrites","Index",null,1),
                new MenuItems("A012","List Of Broker","Brokers", "Broker","Index","01",2),
                new MenuItems("A013","List Of InsuranceType","InsuranceType", "InsuranceTypes","Index",null,1),
                new MenuItems("A014","List Of InsuranceSubType","InsuranceType", "InsuranceTypes","InsuranceSubTypeIndex",null,1),
                //new MenuItems("A015","Authentication","Authentication", "Auth","Index","01",1),
                //new MenuItems("A016","Home","Home", "Home","Index","01",1),
                new MenuItems("A017","Create Insurance","Insurance", "Insurance","CreateRequest1",null,1),
                //new MenuItems("A018","Create External User","External", "Users","CreateExternalUser","01",1),
                //new MenuItems("A019","Create Admin User","Admin", "Users","CreateExternalUser","01",1),
                //new MenuItems("A020","Create Roles","Admin", "Roles","CreateRole","01",2),
                 new MenuItems("A021","Create Underwrites","Underwrites", "Underwrites","CreateUnderwriter",null,1),
                 new MenuItems("A022","Create Broker","Brokers", "Broker","CreateBroker",null,2),
                 new MenuItems("A023","Create InsuranceType","InsuranceType", "InsuranceTypes","CreateInsurance",null,1),
                 new MenuItems("A024","Create InsuranceSubType","InsuranceType", "InsuranceTypes","CreateInsuranceSubType",null,1),
                  new MenuItems("A025","List Of Broker InsuranceType","Brokers", "Broker","BrokerInsuranceIndex","01",2),
                new MenuItems("A026","List Of Broker InsuranceSubType","Brokers", "Broker","BrokerInsuranceSubTypeIndex","01",2),
                new MenuItems("A027","Initaitor Requests","Insurance", "Insurance","GetAllRequest",null,8),
                new MenuItems("A028","Features","Features", "Insurance","ViewFeature",null,8),

                //new MenuItems("A025","Create Broker InsuranceType","Brokers", "Broker","CreateBrokerInsuranceType",null,2),
                 //new MenuItems("A026","Create Broker InsuranceSubType","Brokers", "Broker","CreateBrokerInsuranceSubType",null,2),
               //  new MenuItems("A025","Assign ContractID","External", "Users","CreateExternalUser","01",1),
                //new MenuItems("A026","Create Admin User","Admin", "Users","CreateExternalUser","01",1),
                //new MenuItems("A027","Create Roles","Admin", "Roles","CreateRole","01",2),
                // new MenuItems("A028","Create Underwrites","Underwrites", "Underwrites","CreateUnderwriter",null,1),
                // new MenuItems("A029","Create Broker","Brokers", "Broker","CreateBroker",null,2),
                // new MenuItems("A030","Create InsuranceType","InsuranceType", "InsuranceTypes","CreateInsurance",null,1),
                // new MenuItems("A031","Create InsuranceSubType","InsuranceSubTypes", "InsuranceSubTypes","CreateInsuranceSub",null,1),

            };
            return _menus;
        }



    }
}
