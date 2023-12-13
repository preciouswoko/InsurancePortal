using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Enums
{
    public enum InsuranceStage
    {
        Initial,
        UnderwriterAssigned,
        CertificateUploaded,
        SetContractId,
        AuthorizeRequest,
        RenewalNeeded,
        ReviewCertificate,
        ApprovedCertificate,
        RejectedCertificate,
        End,
        Due,
        New
    }
}
