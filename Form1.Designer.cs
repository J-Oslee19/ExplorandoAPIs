using System;
using System.Windows.Forms;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal
{
    partial class Form1
    {
        private Label labelMensaje;
        private ListBox listaPokemos;
        private Button buscarGato, limpiarGato;
        private PictureBox imagenGato;
        private PictureBox imagenPokemon;
        private Panel panelInformacion;
        private TextBox textoBuscar;
        private Button botonBuscar;
        private List<string> pokemonList = new List<string>();
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void inicializarComponente()
        {
            SetupUI();
            this.Load += async (s, e) => await cargarListaPokemon();
        }

        private void SetupUI()
        {
            // === Ventana principal ===
            this.Text = "Consumo de APIS";
            this.Size = new Size(1400, 850);
            this.BackColor = Color.Black;

            // === Panel de información ===
            panelInformacion = new Panel();
            panelInformacion.Size = new Size(800, 530);
            panelInformacion.Location = new Point(500, 60);
            panelInformacion.BackColor = Color.Cyan;
            panelInformacion.BorderStyle = BorderStyle.FixedSingle;

            // === Etiqueta de mensaje ===
            labelMensaje = new Label();
            labelMensaje.Text = "Selecciona un Pokémon";
            labelMensaje.Location = new Point(20, 20);
            labelMensaje.AutoSize = true;
            labelMensaje.Font = new Font("Arial", 14, FontStyle.Bold);
            labelMensaje.ForeColor = Color.Black;
            panelInformacion.Controls.Add(labelMensaje);

            // === Imagen del Pokémon ===
            imagenPokemon = new PictureBox();
            imagenPokemon.Location = new Point(150, 100);
            imagenPokemon.Size = new Size(250, 250);
            imagenPokemon.SizeMode = PictureBoxSizeMode.StretchImage;
            imagenPokemon.BorderStyle = BorderStyle.FixedSingle;
            panelInformacion.Controls.Add(imagenPokemon);



            // === Imagen del gato ===
            imagenGato = new PictureBox();
            imagenGato.Location = new Point(450, 100);
            imagenGato.Size = new Size(250, 250);
            imagenGato.SizeMode = PictureBoxSizeMode.StretchImage;
            imagenGato.BorderStyle = BorderStyle.FixedSingle;
            panelInformacion.Controls.Add(imagenGato);

            // === Caja de búsqueda ===
            textoBuscar = new TextBox();
            textoBuscar.Location = new Point(510, 700);
            textoBuscar.Size = new Size(180, 25);
            textoBuscar.Font = new Font("Arial", 10);
            textoBuscar.BackColor = Color.White;
            textoBuscar.TextChanged += (s, e) => listaDeFiltros();
            this.Controls.Add(textoBuscar);

            // === Botón Limpiar búsqueda ===
            limpiarGato = new Button();
            limpiarGato.Text = "Limpiar";
            limpiarGato.Location = new Point(710, 700);
            limpiarGato.Size = new Size(80, 25);
            limpiarGato.ForeColor = Color.White;
            limpiarGato.BackColor = Color.FromArgb(220, 53, 69);
            limpiarGato.FlatStyle = FlatStyle.Flat;
            limpiarGato.Click += (s, e) =>
            {
                textoBuscar.Text = "";
                listaPokemos.Items.Clear();
                listaPokemos.Items.AddRange(pokemonList.ToArray());
            };
            this.Controls.Add(limpiarGato);

            // === Botón Buscar Pokémon ===
            botonBuscar = new Button();
            botonBuscar.Text = "Buscar";
            botonBuscar.Location = new Point(810, 700);
            botonBuscar.Size = new Size(80, 25);
            botonBuscar.ForeColor = Color.White;
            botonBuscar.BackColor = Color.FromArgb(40, 167, 69); 
            botonBuscar.FlatStyle = FlatStyle.Flat;
            botonBuscar.Click += async (s, e) => await buscarPokemon();
            this.Controls.Add(botonBuscar);

            // === Botón Mostrar Gatos ===
            buscarGato = new Button();
            buscarGato.Text = "Obetner Gato API";
            buscarGato.Location = new Point(910, 700);
            buscarGato.Size = new Size(120, 25);
            buscarGato.ForeColor = Color.White;
            buscarGato.BackColor = Color.FromArgb(23, 162, 184); 
            buscarGato.FlatStyle = FlatStyle.Flat;
            buscarGato.Click += async (s, e) => await cargarDatosDeGato();
            this.Controls.Add(buscarGato);

            // === Lista de Pokémons ===
            listaPokemos = new ListBox();
            listaPokemos.Location = new Point(40, 60);
            listaPokemos.Size = new Size(440, 540);
            listaPokemos.Font = new Font("Arial", 10);
            listaPokemos.BackColor = Color.FromArgb(245, 245, 245);
            listaPokemos.ForeColor = Color.FromArgb(33, 37, 41);
            listaPokemos.BorderStyle = BorderStyle.FixedSingle;
            listaPokemos.Items.Add("Cargando...");
            listaPokemos.SelectedIndexChanged += new EventHandler(this.listaPokemos_SelectedIndexChanged);

            this.Controls.Add(listaPokemos);
            this.Controls.Add(panelInformacion);
        }

        private async void listaPokemos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listaPokemos.SelectedItem != null)
            {
                string selectedPokemon = listaPokemos.SelectedItem.ToString().ToLower();
                await cargarDatosDePokemon(selectedPokemon);
            }
        }

        private async Task cargarListaPokemon()
        {
            listaPokemos.Items.Clear();
            listaPokemos.Items.Add("Cargando...");
            using (HttpClient client = new HttpClient())
            {
                string url = "https://pokeapi.co/api/v2/pokemon?limit=50";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    pokemonList = ((IEnumerable<dynamic>)data.results).Select(p => (string)p.name).ToList();

                    pokemonList.Sort();

                    listaPokemos.Items.Clear();
                    listaPokemos.Items.AddRange(pokemonList.ToArray());
                }
            }
        }

        private void listaDeFiltros()
        {
            string filter = textoBuscar.Text.ToLower();
            var filteredList = pokemonList.Where(p => p.Contains(filter)).ToList();
            listaPokemos.Items.Clear();
            listaPokemos.Items.AddRange(filteredList.ToArray());
        }

        private async Task cargarDatosDePokemon(string pokemonName)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://pokeapi.co/api/v2/pokemon/{pokemonName}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    string imageUrl = data.sprites.front_default;
                    labelMensaje.Text = $"Seleccionaste: {pokemonName.ToUpper()}";

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        imagenPokemon.Load(imageUrl);
                    }
                    else
                    {
                        imagenPokemon.Image = null;
                        labelMensaje.Text += " (Imagen no disponible)";
                    }
                }
                else
                {
                    labelMensaje.Text = "Error al cargar datos.";
                    imagenPokemon.Image = null;
                }
            }
        }
        private async Task buscarPokemon()
        {
            string searchQuery = textoBuscar.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchQuery) && searchQuery != "buscar...")
            {
                await cargarDatosDePokemon(searchQuery);
            }
            else
            {
                labelMensaje.Text = "Por favor, ingresa un nombre válido para buscar.";
            }
        }

        private async Task cargarDatosDeGato()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.thecatapi.com/v1/images/search";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    string imageUrl = data[0].url;

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        imagenGato.Load(imageUrl);
                    }
                    else
                    {
                        labelMensaje.Text = "No se pudo cargar.";
                        imagenGato.Image = null;
                    }
                }
                else
                {
                    labelMensaje.Text = "Error al cargar datos.";
                    imagenGato.Image = null;
                }
            }
        }
    

        #endregion
    }
}
