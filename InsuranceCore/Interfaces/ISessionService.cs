using System;
using System.Collections.Generic;
using System.Text;
using InsuranceCore.DTO;

namespace InsuranceCore.Interfaces
{
    public interface ISessionService
    {
        void Set<T>(string key, T value);
        T Get<T>(string key);
        void Clear(string key);
    }
}
