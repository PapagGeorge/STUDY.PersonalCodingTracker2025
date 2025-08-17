﻿using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repoitories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if(expiredTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string ipAddress)
        {
            var activeTokens = await GetActiveTokensByUserIdAsync(userId);

            foreach(var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
            }

            if(activeTokens.Any())
            {
                _context.RefreshTokens.UpdateRange(activeTokens);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}