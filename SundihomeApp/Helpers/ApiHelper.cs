using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities.Response;
using SundihomeApp.Models;
using SundihomeApp.Settings;
using Xamarin.Essentials;

namespace SundihomeApp.Helpers
{
    public class ApiHelper
    {
        public async static Task<ApiResponse> Login(string Username, string Password)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            var data = new Dictionary<string, string>();
            data["Username"] = Username;
            data["Password"] = Password;
            ApiResponse loginResponse = await ApiHelper.Post("api/auth/login", data);
            return loginResponse;
        }

        public async static Task<ApiResponse> LoginFacebook(string email, string facebookId)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            ApiResponse loginResponse = await ApiHelper.Post("api/auth/facebooklogin", new
            {
                Email = email,
                FacebookId = facebookId
            });
            return loginResponse;
        }

        public async static Task<ApiResponse> LoginGoogle(string email, string googleId)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            ApiResponse loginResponse = await Post("api/auth/facebooklogin", new
            {
                Email = email,
                GoogleId = googleId
            });
            return loginResponse;
        }

        public async static Task<ApiResponse> LoginZalo(string zaloId)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            ApiResponse loginResponse = await Post("api/auth/facebooklogin", new
            {
                ZaloId = zaloId,
            });
            return loginResponse;
        }

        public async static Task<ApiResponse> Get<T>(string Path, bool Authenticate = false, bool RefreshToken = true) where T : class
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }

            try
            {
                var client = BsdHttpClient.Instance();
                ApiResponse res = new ApiResponse();
                if (Authenticate)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserLogged.AccessToken);
                }
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(Path);

                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    res.IsSuccess = true;
                    res.Content = JsonConvert.DeserializeObject<T>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    res.IsSuccess = true;
                    var body = await response.Content.ReadAsStringAsync();
                    res.Content = JsonConvert.DeserializeObject<T>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //if (response.Headers.Contains("Token-Expired"))
                    //{

                    //}
                    //if (UserLogged.IsLogged && Authenticate && RefreshToken)
                    //{
                    //    ApiResponse loginResponse;
                    //    if (UserLogged.FacebookId != string.Empty)
                    //    {
                    //        loginResponse = await LoginFacebook(UserLogged.Email, UserLogged.FacebookId);
                    //    }
                    //    else if (UserLogged.GoogleId != string.Empty)
                    //    {
                    //        loginResponse = await LoginGoogle(UserLogged.Email, UserLogged.GoogleId);
                    //    }
                    //    else if (UserLogged.ZaloId != string.Empty)
                    //    {
                    //        loginResponse = await LoginZalo(UserLogged.ZaloId);
                    //    }
                    //    else
                    //    {
                    //        loginResponse = await Login(UserLogged.UserName, UserLogged.Password);
                    //    }

                    //    if (loginResponse.IsSuccess)
                    //    {
                    //        AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginResponse.Content.ToString());
                    //        UserLogged.SaveLogin(authResponse);
                    //        res = await Get<T>(Path, true, false);
                    //    }
                    //    else
                    //    {
                    //        res.Message = "Vui long dang nhap lai";
                    //        res.IsSuccess = false;
                    //    }
                    //}
                    //else
                    //{
                    //    // loi dang nhap, co token user pass nhung ko dang nhap duoc.
                    //    res.Message = "Vui long dang nhap lai";
                    //    res.IsSuccess = false;
                    //}
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    res.Message = body;
                    res.IsSuccess = false;
                }
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async static Task<ApiResponse> Post(string Path, object formContent, bool Authenticate = false, bool RefreshToken = true)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            try
            {
                var client = BsdHttpClient.Instance();
                ApiResponse res = new ApiResponse();
                if (Authenticate)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserLogged.AccessToken);
                }
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response;

                if (formContent == null)
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, Path);
                    response = await client.SendAsync(request);
                }
                else
                {
                    string objContent = JsonConvert.SerializeObject(formContent);
                    HttpContent content = new StringContent(objContent, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(Path, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //if (UserLogged.IsLogged && Authenticate && RefreshToken)
                    //{
                    //    ApiResponse loginResponse;
                    //    if (UserLogged.FacebookId != string.Empty)
                    //    {
                    //        loginResponse = await LoginFacebook(UserLogged.Email, UserLogged.FacebookId);
                    //    }
                    //    else if (UserLogged.GoogleId != string.Empty)
                    //    {
                    //        loginResponse = await LoginGoogle(UserLogged.Email, UserLogged.GoogleId);
                    //    }
                    //    else if (UserLogged.ZaloId != string.Empty)
                    //    {
                    //        loginResponse = await LoginZalo(UserLogged.ZaloId);
                    //    }
                    //    else
                    //    {
                    //        loginResponse = await Login(UserLogged.UserName, UserLogged.Password);
                    //    }
                    //    if (loginResponse.IsSuccess)
                    //    {
                    //        AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginResponse.Content.ToString());
                    //        UserLogged.SaveLogin(authResponse);
                    //        res = await Post(Path, formContent, true, false);
                    //    }
                    //    else
                    //    {
                    //        res.Message = "Vui long dang nhap lai";
                    //        res.IsSuccess = false;
                    //    }
                    //}
                    //else
                    //{
                    //    // loi dang nhap, co token user pass nhung ko dang nhap duoc.
                    //    res.Message = "Vui long dang nhap lai";
                    //    res.IsSuccess = false;
                    //}
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    res.Message = "Lỗi";
                    res.IsSuccess = false;
                }
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async static Task<ApiResponse> Put(string Path, object formContent, bool Authenticate = false, bool RefreshToken = true)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            try
            {
                var client = BsdHttpClient.Instance();
                ApiResponse res = new ApiResponse();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Authenticate)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserLogged.AccessToken);
                }
                HttpResponseMessage response;

                if (formContent == null)
                {
                    var request = new HttpRequestMessage(HttpMethod.Put, Path);
                    response = await client.SendAsync(request);
                }
                else
                {
                    string objContent = JsonConvert.SerializeObject(formContent);
                    HttpContent content = new StringContent(objContent, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(Path, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //if (UserLogged.IsLogged && Authenticate && RefreshToken)
                    //{
                    //    ApiResponse loginResponse;
                    //    if (UserLogged.FacebookId != string.Empty)
                    //    {
                    //        loginResponse = await LoginFacebook(UserLogged.Email, UserLogged.FacebookId);
                    //    }
                    //    else if (UserLogged.GoogleId != string.Empty)
                    //    {
                    //        loginResponse = await LoginGoogle(UserLogged.Email, UserLogged.GoogleId);
                    //    }
                    //    else if (UserLogged.ZaloId != string.Empty)
                    //    {
                    //        loginResponse = await LoginZalo(UserLogged.ZaloId);
                    //    }
                    //    else
                    //    {
                    //        loginResponse = await Login(UserLogged.UserName, UserLogged.Password);
                    //    }

                    //    if (loginResponse.IsSuccess)
                    //    {
                    //        AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginResponse.Content.ToString());
                    //        UserLogged.SaveLogin(authResponse);
                    //        res = await Put(Path, formContent, true, false);
                    //    }
                    //    else
                    //    {
                    //        res.Message = "Vui long dang nhap lai";
                    //        res.IsSuccess = false;
                    //    }
                    //}
                    //else
                    //{
                    //    // loi dang nhap, co token user pass nhung ko dang nhap duoc.
                    //    res.Message = "Vui long dang nhap lai";
                    //    res.IsSuccess = false;
                    //}
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    res.Message = "Lỗi";
                    res.IsSuccess = false;
                }
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async static Task<ApiResponse> Delete(string Path, bool RefreshToken = true)
        {
            var network = Connectivity.NetworkAccess;
            if (network != NetworkAccess.Internet)
            {
                return new ApiResponse(false, null, "Không có kết nối Internet");
            }
            try
            {
                var client = BsdHttpClient.Instance();
                ApiResponse res = new ApiResponse();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserLogged.AccessToken);
                HttpResponseMessage response = await client.DeleteAsync(Path);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse>(body);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //if (UserLogged.IsLogged && RefreshToken)
                    //{
                    //    ApiResponse loginResponse;
                    //    if (UserLogged.FacebookId != string.Empty)
                    //    {
                    //        loginResponse = await LoginFacebook(UserLogged.Email, UserLogged.FacebookId);
                    //    }
                    //    else if (UserLogged.GoogleId != string.Empty)
                    //    {
                    //        loginResponse = await LoginGoogle(UserLogged.Email, UserLogged.GoogleId);
                    //    }
                    //    else if (UserLogged.ZaloId != string.Empty)
                    //    {
                    //        loginResponse = await LoginZalo(UserLogged.ZaloId);
                    //    }
                    //    else
                    //    {
                    //        loginResponse = await Login(UserLogged.UserName, UserLogged.Password);
                    //    }
                    //    if (loginResponse.IsSuccess)
                    //    {
                    //        AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(loginResponse.Content.ToString());
                    //        UserLogged.SaveLogin(authResponse);
                    //        res = await Delete(Path, false);
                    //    }
                    //    else
                    //    {
                    //        res.Message = "Vui long dang nhap lai";
                    //        res.IsSuccess = false;
                    //    }
                    //}
                    //else
                    //{
                    //    // loi dang nhap, co token user pass nhung ko dang nhap duoc.
                    //    res.Message = "Vui long dang nhap lai";
                    //    res.IsSuccess = false;
                    //}
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    res.Message = "Lỗi";
                    res.IsSuccess = false;
                }
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
