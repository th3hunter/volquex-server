using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace Volquex.Models
{
        /// <summary>
        /// Database       : volquex
        /// Data Source    : localhost
        /// Server Version : 10.4
        /// </summary>
        public partial class VolquexDB : LinqToDB.Data.DataConnection
        {
                public ITable<Estados> Estados { get { return this.GetTable<Estados>(); } }
                public ITable<Materiales> Materiales { get { return this.GetTable<Materiales>(); } }
                public ITable<Parametros> Parametros { get { return this.GetTable<Parametros>(); } }
                public ITable<Presentaciones> Presentaciones { get { return this.GetTable<Presentaciones>(); } }
                public ITable<Sesiones> Sesiones { get { return this.GetTable<Sesiones>(); } }
                public ITable<Usuarios> Usuarios { get { return this.GetTable<Usuarios>(); } }
                public ITable<Usuarios_Loc> Usuarios_Loc { get { return this.GetTable<Usuarios_Loc>(); } }
                public ITable<Viajes> Viajes { get { return this.GetTable<Viajes>(); } }
                public ITable<Viajes_Bit> Viajes_Bit { get { return this.GetTable<Viajes_Bit>(); } }
                public ITable<Viajes_Mat> Viajes_Mat { get { return this.GetTable<Viajes_Mat>(); } }
                public ITable<Viajes_Volq> Viajes_Volq { get { return this.GetTable<Viajes_Volq>(); } }
                public ITable<Volquetas> Volquetas { get { return this.GetTable<Volquetas>(); } }
                public ITable<Volquetas_Conduc> Volquetas_Conduc { get { return this.GetTable<Volquetas_Conduc>(); } }

                public VolquexDB()
                {
                }

                public VolquexDB(string configuration)
                        : base(configuration)
                {
                }
        }

        [Table(Schema = "public", Name = "estados")]
        public partial class Estados
        {
                [PrimaryKey, NotNull] public string TablaId { get; set; } // numeric
                [PrimaryKey, NotNull] public string EstadoId { get; set; } // character varying(120)
                [Column, NotNull] public string EstDsc { get; set; } // integer
                [Column, NotNull] public string EstVal { get; set; } // integer
                [Column, NotNull] public int EstOrden { get; set; } // integer
                [Column, NotNull] public string EstEst { get; set; } // integer

                // Atributos no presentes en la B/D
                public string TablaDsc { get; set; }

        }

        [Table(Schema = "public", Name = "materiales")]
        public partial class Materiales
        {
                [PrimaryKey, NotNull] public decimal MaterialId { get; set; } // numeric
                [Column, NotNull] public string MatDsc { get; set; } // character varying(120)
                [Column, NotNull] public int Presentacion { get; set; } // integer
                [Column, NotNull] public decimal MatPrecio { get; set; } // money
                [Column, NotNull] public string MatEst { get; set; } // character varying(1)

                // Atributos no presentes en la B/D
                public string PresDsc { get; set; }
                public string EstDsc { get; set; }

                #region Associations

                /// <summary>
                /// fk_materiales_presentaciones_1
                /// </summary>
                [Association(ThisKey = "Presentacion", OtherKey = "PresentacionId", CanBeNull = false)]
                public Presentaciones fkpresentaciones1 { get; set; }

                /// <summary>
                /// fk_viajes_mat_materiales_1_BackReference
                /// </summary>
                [Association(ThisKey = "MaterialId", OtherKey = "MaterialId", CanBeNull = false)]
                public IEnumerable<Viajes_Mat> fkviajesmat1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "parametros")]
        public partial class Parametros
        {
                [PrimaryKey, NotNull] public string ParametroId { get; set; } // string
                [Column, NotNull] public string ParamDsc { get; set; } // integer
                [Column, NotNull] public string ParamVal { get; set; } // integer
                [Column, NotNull] public string ParamEst { get; set; } // integer

        }

        [Table(Schema = "public", Name = "presentaciones")]
        public partial class Presentaciones
        {
                [PrimaryKey, NotNull] public int PresentacionId { get; set; } // integer
                [Column, NotNull] public string PresDsc { get; set; } // character varying(30)
                [Column, NotNull] public string PresEst { get; set; } // character varying(1)

                // Atributos no presentes en la B/D
                public string EstDsc { get; set; }

                #region Associations

                /// <summary>
                /// fk_materiales_presentaciones_1_BackReference
                /// </summary>
                [Association(ThisKey = "PresentacionId", OtherKey = "Presentacion", CanBeNull = false)]
                public IEnumerable<Materiales> fkmateriales1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "sesiones")]
        public partial class Sesiones
        {
                [PrimaryKey, NotNull] public string SesionId { get; set; } // integer
                [Column, NotNull] public DateTime SesionExpira { get; set; } // timestamp (6) without time zone
                [Column, NotNull] public string SesionTipo { get; set; } // timestamp (6) without time zone
                [Column, NotNull] public decimal UsuarioId { get; set; } // numeric

                #region Associations

                /// <summary>
                /// fk_sesiones_usuarios_1
                /// </summary>
                [Association(ThisKey = "SesionId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios fkusuarios1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "usuarios")]
        public partial class Usuarios
        {
                [PrimaryKey, NotNull] public decimal UsuarioId { get; set; } // numeric
                [Column, NotNull] public string UsuNom { get; set; } // character varying(60)
                [Column, NotNull] public string UsuEmail { get; set; } // character varying(40)
                [Column, NotNull] public string UsuCel { get; set; } // character varying(15)
                [Column, NotNull] public string UsuTipo { get; set; } // character varying(3)
                [Column, NotNull] public string UsuEst { get; set; } // character varying(1)
                [Column, Nullable] public string UsuPassword { get; set; } // character varying(255)
                [Column, Nullable] public int UsuCalificacion { get; set; } // integer
                [Column, Nullable] public decimal UsuNumCalificaciones { get; set; } // numeric
                [Column, Nullable] public string UsuFoto { get; set; } // character varying(255)
                [Column, Nullable] public string UsuTipoFoto { get; set; } // character varying(255)

                // Atributos no presentes en la B/D
                public string EstDsc { get; set; }

                #region Associations

                /// <summary>
                /// fk_sesiones_usuarios_1_BackReference
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "SesionId", CanBeNull = false)]
                public Sesiones fksesiones1 { get; set; }

                /// <summary>
                /// fk_volquetas_conduc_usuarios_1_BackReference
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "UsuarioId", CanBeNull = false)]
                public IEnumerable<Volquetas_Conduc> fkvolquetasconduc1 { get; set; }

                /// <summary>
                /// fk_viajes_usuarios_1_BackReference
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "ClienteId", CanBeNull = false)]
                public IEnumerable<Viajes> fkviajes1 { get; set; }

                /// <summary>
                /// fk_viajes_usuarios_2_BackReference
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "ConductorId", CanBeNull = false)]
                public IEnumerable<Viajes> fkviajes2 { get; set; }

                /// <summary>
                /// fk_usuarios_loc_usuarios_1_BackReference
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios_Loc fkloc1 { get; set; }

                /// <summary>
                /// fk_volquetas_usuarios_1_BackReference
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "ConductorId", CanBeNull = false)]
                public IEnumerable<Volquetas> fkvolquetas1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "usuarios_loc")]
        public partial class Usuarios_Loc
        {
                [PrimaryKey, NotNull] public decimal UsuarioId { get; set; } // numeric
                [Column, NotNull] public int UsuLocId { get; set; } // integer
                [Column, NotNull] public string UsuLocLat { get; set; } // string precision
                [Column, NotNull] public string UsuLocLon { get; set; } // string precision
                [Column, NotNull] public string UsuLocDsc { get; set; } // character varying(120)

                #region Associations

                /// <summary>
                /// fk_usuarios_loc_usuarios_1
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios fkusuarioslocusuarios1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "viajes")]
        public partial class Viajes
        {
                [PrimaryKey, NotNull] public decimal ViajeId { get; set; } // numeric
                [Column, NotNull] public DateTime ViaFch { get; set; } // date
                [Column, NotNull] public DateTime ViaFchHr { get; set; } // timestamp (6) without time zone
                [Column, NotNull] public DateTime ViaFchHrLlegada { get; set; } // timestamp (6) without time zone
                [Column, NotNull] public decimal ClienteId { get; set; } // numeric
                [Column, Nullable] public string ViaOrigenLat { get; set; } // double precision
                [Column, Nullable] public string ViaOrigenLon { get; set; } // double precision
                [Column, Nullable] public string ViaDestinoLat { get; set; } // double precision
                [Column, Nullable] public string ViaDestinoLon { get; set; } // double precision
                [Column, Nullable] public string ViaNombreDestino { get; set; } // double precision
                [Column, Nullable] public int? VolquetaId { get; set; } // integer
                [Column, Nullable] public decimal ViaTotal { get; set; } // money
                [Column, NotNull] public string ViaEst { get; set; } // character varying(1)
                [Column, Nullable] public decimal? ConductorId { get; set; } // numeric
                [Column, Nullable] public int? ViaCalificacionCliente { get; set; } // integer
                [Column, Nullable] public int? ViaCalificacionConductor { get; set; } // integer
                [Column, Nullable] public decimal ViaValorFlete { get; set; } // money
                [Column, Nullable] public string ViaComentario { get; set; } // character varying(255)

                #region Associations

                // Atributos no presentes en la B/D
                public string CliNom { get; set; }
                public string ConducNom { get; set; }
                public string ConducFoto { get; set; }
                public string ConducTipoFoto { get; set; }
                public string VolqDsc { get; set; }
                public string VolqLat { get; set; }
                public string VolqLon { get; set; }
                public string VolqPlaca { get; set; }
                public decimal VolqCapacidad { get; set; }
                public string EstDsc { get; set; }
                public Viajes_Mat[] Materiales { get; set; }

                /// <summary>
                /// fk_viajes_volquetas_1
                /// </summary>
                [Association(ThisKey = "VolquetaId", OtherKey = "VolquetaId", CanBeNull = true)]
                public Volquetas fkvolquetas1 { get; set; }

                /// <summary>
                /// fk_viajes_usuarios_1
                /// </summary>
                [Association(ThisKey = "ClienteId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios fkcliente1 { get; set; }

                /// <summary>
                /// fk_viajes_usuarios_2
                /// </summary>
                [Association(ThisKey = "ConductorId", OtherKey = "UsuarioId", CanBeNull = true)]
                public Usuarios fkconductor1 { get; set; }

                /// <summary>
                /// fk_viajes_bit_viajes_1_BackReference
                /// </summary>
                [Association(ThisKey = "ViajeId", OtherKey = "ViajeId", CanBeNull = false)]
                public Viajes_Bit fkbit1 { get; set; }

                /// <summary>
                /// fk_viajes_volq_viajes_1_BackReference
                /// </summary>
                [Association(ThisKey = "ViajeId", OtherKey = "ViajeId", CanBeNull = false)]
                public Viajes_Volq fkvolq1 { get; set; }

                /// <summary>
                /// fk_viajes_mat_viajes_1_BackReference
                /// </summary>
                [Association(ThisKey = "ViajeId", OtherKey = "ViajeId", CanBeNull = false)]
                public Viajes_Mat fkmat1 { get; set; }

                #endregion

                public static Viajes Build(Viajes viaje, Usuarios cliente, Usuarios conductor, Volquetas volqueta)
                {
                        if (viaje != null)
                        {
                                viaje.fkcliente1 = cliente;
                                viaje.fkconductor1 = conductor;
                                viaje.fkvolquetas1 = volqueta;
                        }

                        return viaje;
                }
        }

        [Table(Schema = "public", Name = "viajes_bit")]
        public partial class Viajes_Bit
        {
                [PrimaryKey, NotNull] public decimal ViajeId { get; set; }
                [Column, NotNull] public int ViajeBitacoraId { get; set; }
                [Column, NotNull] public DateTime ViaBitFchHr { get; set; }
                [Column, NotNull] public string ViaBitEst { get; set; }

                // Atributos no presentes en la B/D
                public string EstDsc { get; set; }

                #region Associations

                /// <summary>
                /// fk_viajes_bit_viajes_1
                /// </summary>
                [Association(ThisKey = "ViajeId", OtherKey = "ViajeId", CanBeNull = false)]
                public Viajes fkviajesbitviajes1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "viajes_mat")]
        public partial class Viajes_Mat
        {
                [PrimaryKey, NotNull] public decimal ViajeId { get; set; } // numeric
                [Column, NotNull] public int ViaMatId { get; set; } // integer
                [Column, NotNull] public decimal MaterialId { get; set; } // numeric
                [Column, NotNull] public decimal ViaMatCantidad { get; set; } // numeric(7,2)
                [Column, NotNull] public decimal ViaMatPrecio { get; set; } // money
                [Column, NotNull] public decimal ViaMatImporte { get; set; } // money

                #region Associations

                // Atributos no presentes en la B/D
                public string MatDsc { get; set; }
                public string PresDsc { get; set; }

                /// <summary>
                /// fk_viajes_mat_materiales_1
                /// </summary>
                [Association(ThisKey = "MaterialId", OtherKey = "MaterialId", CanBeNull = false)]
                public Materiales fkviajesmatmateriales1 { get; set; }

                /// <summary>
                /// fk_viajes_mat_viajes_1
                /// </summary>
                [Association(ThisKey = "ViajeId", OtherKey = "ViajeId", CanBeNull = false)]
                public Viajes fkviajesmatviajes1 { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "viajes_volq")]
        public partial class Viajes_Volq
        {
                [PrimaryKey, NotNull] public decimal ViajeId { get; set; } // numeric
                [Column, NotNull] public int ViajeVolquetaId { get; set; } // integer
                [Column, NotNull] public int VolquetaId { get; set; } // integer
                [Column, NotNull] public char ViaVolqTipoDscto { get; set; } // character varying(1)
                [Column, NotNull] public decimal ViaVolqDscto { get; set; } // money
                [Column, NotNull] public decimal ViaVolqOferta { get; set; } // money
                [Column, NotNull] public string ViaVolqEst { get; set; } // character varying(1)
                [Column, NotNull] public decimal ConductorId { get; set; } // character varying(1)
                [Column, NotNull] public string ViaVolqEstViaje { get; set; } // character varying(1)

                #region Associations

                // Atributos no presentes en la B/D
                public string VolqDsc { get; set; }
                public string VolqPlaca { get; set; }
                public decimal VolqCapacidad { get; set; }
                public string ConducNom { get; set; }
                public string ConducFoto { get; set; }
                public string ConducTipoFoto { get; set; }
                public string EstDsc { get; set; }

                /// <summary>
                /// fk_viajes_volq_viajes_1
                /// </summary>
                [Association(ThisKey = "ViajeId", OtherKey = "ViajeId", CanBeNull = false)]
                public Viajes fkviajesvolqviajes1 { get; set; }

                /// <summary>
                /// fk_viajes_volq_volquetas_1
                /// </summary>
                [Association(ThisKey = "VolquetaId", OtherKey = "VolquetaId", CanBeNull = false)]
                public Volquetas fkviajesvolqvolquetas1 { get; set; }

                /// <summary>
                /// fk_viajes_volq_conductor
                /// </summary>
                [Association(ThisKey = "ConductorId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios fkviajesvolqconductor { get; set; }

                #endregion
        }

        [Table(Schema = "public", Name = "volquetas")]
        public partial class Volquetas
        {
                [PrimaryKey, NotNull] public int VolquetaId { get; set; } // integer
                [Column, NotNull] public string VolqDsc { get; set; } // character varying(60)
                [Column, NotNull] public string VolqPlaca { get; set; } // character varying(10)
                [Column, Nullable] public string VolqLat { get; set; } // double precision
                [Column, Nullable] public string VolqLon { get; set; } // double precision
                [Column, NotNull] public decimal VolqCapacidad { get; set; } // numeric(7,2)
                [Column, NotNull] public decimal ConductorId { get; set; } // numeric
                [Column, NotNull] public string VolqEst { get; set; } // character varying(1)
                [Column, NotNull] public int VolqCalificacion { get; set; } // integer
                [Column, NotNull] public decimal VolqNumCalificaciones { get; set; } // numeric

                #region Associations

                // Atributos no presentes en la B/D
                public string ConducNom { get; set; }
                public string ConducFoto { get; set; }
                public string ConducTipoFoto { get; set; }
                public string EstDsc { get; set; }

                /// <summary>
                /// fk_volquetas_usuarios_1
                /// </summary>
                [Association(ThisKey = "ConductorId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios fkusuarios1 { get; set; }

                /// <summary>
                /// fk_viajes_volq_volquetas_1_BackReference
                /// </summary>
                [Association(ThisKey = "VolquetaId", OtherKey = "VolquetaId", CanBeNull = false)]
                public IEnumerable<Viajes_Volq> fkviajesvolq1 { get; set; }

                /// <summary>
                /// fk_volquetas_conduc_volquetas_1_BackReference
                /// </summary>
                [Association(ThisKey = "VolquetaId", OtherKey = "VolquetaId", CanBeNull = false)]
                public Volquetas_Conduc fkconduc1 { get; set; }

                /// <summary>
                /// fk_viajes_volquetas_1_BackReference
                /// </summary>
                [Association(ThisKey = "VolquetaId", OtherKey = "VolquetaId", CanBeNull = false)]
                public IEnumerable<Viajes> fkviajes1 { get; set; }

                #endregion

                public static Volquetas Build(Volquetas volqueta, Usuarios conductor)
                {
                        if (volqueta != null)
                                volqueta.fkusuarios1 = conductor;

                        return volqueta;
                }
        }

        [Table(Schema = "public", Name = "volquetas_conduc")]
        public partial class Volquetas_Conduc
        {
                [PrimaryKey, NotNull] public int VolquetaId { get; set; } // integer
                [Column, NotNull] public decimal UsuarioId { get; set; } // numeric

                #region Associations

                /// <summary>
                /// fk_volquetas_conduc_volquetas_1
                /// </summary>
                [Association(ThisKey = "VolquetaId", OtherKey = "VolquetaId", CanBeNull = false)]
                public Volquetas fkvolquetasconducvolquetas1 { get; set; }

                /// <summary>
                /// fk_volquetas_conduc_usuarios_1
                /// </summary>
                [Association(ThisKey = "UsuarioId", OtherKey = "UsuarioId", CanBeNull = false)]
                public Usuarios fkvolquetasconducusuarios1 { get; set; }

                #endregion
        }

        public static partial class tableExtensions
        {
            public static Materiales Find(this ITable<Materiales> table, decimal MaterialId)
            {
                    return table.FirstOrDefault(t =>
                            t.MaterialId == MaterialId);
            }

            public static Presentaciones Find(this ITable<Presentaciones> table, int PresentacionId)
            {
                    return table.FirstOrDefault(t =>
                            t.PresentacionId == PresentacionId);
            }

            public static Sesiones Find(this ITable<Sesiones> table, string SesionId)
            {
                    return table.FirstOrDefault(t =>
                            t.SesionId == SesionId);
            }

            public static Usuarios Find(this ITable<Usuarios> table, decimal UsuarioId)
            {
                    return table.FirstOrDefault(t =>
                            t.UsuarioId == UsuarioId);
            }

            public static Usuarios_Loc Find(this ITable<Usuarios_Loc> table, decimal UsuarioId)
            {
                    return table.FirstOrDefault(t =>
                            t.UsuarioId == UsuarioId);
            }

            public static Viajes Find(this ITable<Viajes> table, decimal ViajeId)
            {
                    return table.FirstOrDefault(t =>
                            t.ViajeId == ViajeId);
            }

            public static Viajes_Mat Find(this ITable<Viajes_Mat> table, decimal ViajeId)
            {
                    return table.FirstOrDefault(t =>
                            t.ViajeId == ViajeId);
            }

            public static Viajes_Volq Find(this ITable<Viajes_Volq> table, decimal ViajeId)
            {
                    return table.FirstOrDefault(t =>
                            t.ViajeId == ViajeId);
            }

            public static Volquetas Find(this ITable<Volquetas> table, int VolquetaId)
            {
                    return table.FirstOrDefault(t =>
                            t.VolquetaId == VolquetaId);
            }

            public static Volquetas_Conduc Find(this ITable<Volquetas_Conduc> table, int VolquetaId)
            {
                    return table.FirstOrDefault(t =>
                            t.VolquetaId == VolquetaId);
            }
        }
}
