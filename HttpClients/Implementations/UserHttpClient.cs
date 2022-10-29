﻿using Domain.DTOs;
using Domain.Models;
using HttpClients.ClientInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpClients.Implementations
{
    public class UserHttpClient : IUserService
    {
        private readonly HttpClient client;

        public UserHttpClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<User> CreateAsync(UserCreateDTO dto)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/users", dto);
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }

            User user = JsonSerializer.Deserialize<User>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return user;
        }

        public async Task<UserBasicDTO> GetByIdAsync(int id)
        {

            HttpResponseMessage response = await client.GetAsync($"/api/users/{id}");
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }

            UserBasicDTO profile = JsonSerializer.Deserialize<UserBasicDTO>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return profile;
        }

        public async Task<ICollection<User>> GetAsync(string? userName, string? email, string? fullName)
        {
            string query = ConstructQuery(userName, email, fullName);

            HttpResponseMessage response = await client.GetAsync("/api/users" + query);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            ICollection<User> users = JsonSerializer.Deserialize<ICollection<User>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return users;
        }

        private static string ConstructQuery(string? userName, string? email, string? fullName)
        {
            string query = "";
            if (!string.IsNullOrEmpty(userName))
            {
                query += $"?username={userName}";
            }

            if (!string.IsNullOrEmpty(email))
            {
                query += string.IsNullOrEmpty(query) ? "?" : "&";
                query += $"email={email}";
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                query += string.IsNullOrEmpty(query) ? "?" : "&";
                query += $"fullname={fullName}";
            }

            return query; 
        }

        public async Task DeleteAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"/api/users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }

        }

        public async Task UpdateAsync(UserUpdateDTO dto)
        {
            string dtoAsJson = JsonSerializer.Serialize(dto);
            StringContent body = new StringContent(dtoAsJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PatchAsync("/api/users", body);
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception(content);
            }
        }
    }
}
