using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Curso.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder
                .HasOne(p=>p.Governador)
                .WithOne(p=>p.Estado)
                .HasForeignKey<Governador>(p=>p.EstadoReference);

            builder.Navigation(p=>p.Governador).AutoInclude();

            builder.HasMany(d=> d.Cidades)
            .WithOne(p=> p.Estado)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}