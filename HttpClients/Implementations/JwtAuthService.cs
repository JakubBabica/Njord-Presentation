﻿using Domain.DTOs;
using Domain.DTOs.Member;
using Domain.Models;
using HttpClients.ClientInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpClients.Implementations
{
    public class JwtAuthService : IAuthService
    {
        private readonly HttpClient client;
        public static string? Jwt { get; private set; } = "";

        public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; } = null!;

        public JwtAuthService(HttpClient client)
        {
            this.client = client;
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            string payload = jwt.Split('.')[1];
            byte[] jsonBytes = ParseBase64WithoutPadding(payload);
            Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }

        private static ClaimsPrincipal CreateClaimsPrincipal()
        {
            if (string.IsNullOrEmpty(Jwt))
            {
                return new ClaimsPrincipal();
            }

            IEnumerable<Claim> claims = ParseClaimsFromJwt(Jwt);

            ClaimsIdentity identity = new(claims, "jwt");

            ClaimsPrincipal principal = new(identity);
            return principal;
        }

        public async Task LoginAsync(MemberLoginDTO dto)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7033/api/auth", dto);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(responseContent);
            }

            string token = responseContent;
            Jwt = token;

            ClaimsPrincipal principal = CreateClaimsPrincipal();

            OnAuthStateChanged.Invoke(principal);
        }

        public Task LogoutAsync()
        {
            Jwt = null;
            ClaimsPrincipal principal = new();
            OnAuthStateChanged.Invoke(principal);
            return Task.CompletedTask;
        }

        public Task RegisterAsync(MemberEntity user)
        {
            throw new NotImplementedException();
        }

        public Task<ClaimsPrincipal> GetAuthAsync()
        {
            ClaimsPrincipal principal = CreateClaimsPrincipal();
            return Task.FromResult(principal);
        }
    }
}
