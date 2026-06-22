<%@ Page Title="Detalle del Producto" Language="C#" MasterPageFile="~/PrincipalMaster.master"
    AutoEventWireup="true" CodeBehind="ProductoDetalle.aspx.cs" Inherits="Monolito_4bm.ProductoDetalle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800&display=swap" rel="stylesheet" />
    <style>
        body {
            font-family: 'Poppins', sans-serif;
            background: #f8f9fc;
        }

        .product-detail-wrapper {
            background: white;
            border-radius: 24px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.05);
            overflow: hidden;
            margin-bottom: 2rem;
        }

        .product-gallery {
            background: #fff;
            padding: 1.5rem;
        }

        .carousel-inner img {
            max-height: 400px;
            object-fit: contain;
            cursor: pointer;
        }

        .thumbnail-list {
            display: flex;
            gap: 12px;
            margin-top: 20px;
            flex-wrap: wrap;
        }

        .thumbnail {
            width: 80px;
            height: 80px;
            border-radius: 12px;
            overflow: hidden;
            cursor: pointer;
            border: 2px solid transparent;
            transition: all 0.2s;
            background: #f1f3f5;
        }

        .thumbnail img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .thumbnail.active {
            border-color: #0f3460;
            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
        }

        .product-info {
            padding: 1.5rem;
        }

        .brand {
            font-size: 0.85rem;
            text-transform: uppercase;
            letter-spacing: 1px;
            color: #6c8ebf;
            font-weight: 600;
        }

        .product-title {
            font-size: 1.8rem;
            font-weight: 700;
            color: #1e2f3e;
            margin: 0.5rem 0;
        }

        .price-container {
            background: #f0f4ff;
            padding: 1rem;
            border-radius: 20px;
            margin: 1rem 0;
        }

        .current-price {
            font-size: 2.2rem;
            font-weight: 800;
            color: #0f3460;
        }

        .old-price {
            font-size: 1.1rem;
            color: #999;
            text-decoration: line-through;
            margin-left: 12px;
        }

        .installments {
            font-size: 0.9rem;
            color: #2e7d32;
            font-weight: 500;
        }

        .stock-badge {
            display: inline-block;
            background: #e8f5e9;
            color: #2e7d32;
            padding: 6px 14px;
            border-radius: 40px;
            font-size: 0.8rem;
            font-weight: 600;
        }

        .payment-icons {
            display: flex;
            gap: 15px;
            margin: 15px 0;
        }

        .payment-icons i {
            font-size: 2rem;
            color: #4a6a8b;
        }

        .btn-whatsapp {
            background: #25D366;
            color: white;
            font-weight: 600;
            padding: 12px 20px;
            border-radius: 50px;
            width: 100%;
            transition: 0.2s;
            border: none;
        }

        .btn-whatsapp:hover {
            background: #128C7E;
            transform: scale(1.02);
        }

        .info-section {
            background: #f8f9fc;
            border-radius: 20px;
            padding: 1.2rem;
            margin-top: 1rem;
        }

        .info-title {
            font-weight: 700;
            margin-bottom: 10px;
            font-size: 1rem;
        }

        .stats-chart {
            max-width: 100%;
            margin-top: 1rem;
            background: white;
            border-radius: 20px;
            padding: 1rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfProId" runat="server" Value="0" />
    <asp:HiddenField ID="hfProductName" runat="server" Value="" />
    <asp:HiddenField ID="hfProductPrice" runat="server" Value="0" />

    <div class="container py-4">
        <asp:Button ID="btnVolver" runat="server" Text="← Volver a productos" CssClass="btn btn-outline-secondary mb-3" OnClick="btnVolver_Click" />

        <div class="product-detail-wrapper">
            <div class="row g-0">
                <!-- COLUMNA GALERÍA (CARRUSEL + MINIATURAS) -->
                <div class="col-md-6">
                    <div class="product-gallery">
                        <!-- CARRUSEL BOOTSTRAP -->
                        <div id="productCarousel" class="carousel slide" data-bs-ride="false" data-bs-interval="false">
                            <div class="carousel-inner">
                                <asp:Repeater ID="rptCarouselItems" runat="server">
                                    <ItemTemplate>
                                        <div class="carousel-item <%# Container.ItemIndex == 0 ? "active" : "" %>">
                                            <img src='<%# ResolveUrl("~/" + Eval("img_ruta")) %>'
                                                 class="d-block w-100"
                                                 data-bs-toggle="modal" data-bs-target="#imageModal"
                                                 onclick="setModalImage(this.src)" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <button class="carousel-control-prev" type="button" data-bs-target="#productCarousel" data-bs-slide="prev">
                                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                                <span class="visually-hidden">Anterior</span>
                            </button>
                            <button class="carousel-control-next" type="button" data-bs-target="#productCarousel" data-bs-slide="next">
                                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                                <span class="visually-hidden">Siguiente</span>
                            </button>
                        </div>

                        <!-- MINIATURAS (sincronizadas con el carrusel) -->
                        <div class="thumbnail-list mt-3">
                            <asp:Repeater ID="rptThumbnails" runat="server">
                                <ItemTemplate>
                                    <div class="thumbnail" data-slide-to='<%# Container.ItemIndex %>'>
                                        <img src='<%# ResolveUrl("~/" + Eval("img_ruta")) %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

                <!-- COLUMNA INFORMACIÓN -->
                <div class="col-md-6">
                    <div class="product-info">
                        <div class="brand">MONOLITO 4BM</div>
                        <h1 class="product-title"><asp:Literal ID="ltlNombre" runat="server" /></h1>
                        <div><span class="stock-badge"><i class="fas fa-check-circle"></i> <asp:Literal ID="ltlStock" runat="server" /> unidades disponibles</span></div>

                        <div class="price-container">
                            <span class="current-price"><asp:Literal ID="ltlPrecio" runat="server" /></span>
                            <span class="old-price"><asp:Literal ID="ltlPrecioAntiguo" runat="server" /></span>
                            <div class="installments">Hasta 6 cuotas sin interés de <strong><asp:Literal ID="ltlCuota" runat="server" /></strong></div>
                        </div>

                        <div class="payment-icons">
                            <i class="fab fa-cc-visa"></i>
                            <i class="fab fa-cc-mastercard"></i>
                            <i class="fab fa-cc-amex"></i>
                            <i class="fab fa-cc-paypal"></i>
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Seleccionar opción</label>
                            <select class="form-select" id="ddlOpcion" runat="server">
                                <option value="solo_armazon">Solo armazón</option>
                                <option value="con_lentes">Armazón + lentes (30% dcto)</option>
                                <option value="descanso">Lentes de descanso</option>
                            </select>
                        </div>

                        <button id="btnWhatsApp" runat="server" class="btn btn-whatsapp" onserverclick="btnWhatsApp_Click">
                            <i class="fab fa-whatsapp me-2"></i> COMPRAR VÍA WHATSAPP
                        </button>

                        <div class="info-section mt-3">
                            <div class="info-title"><i class="fas fa-truck"></i> Envíos</div>
                            <p class="mb-0 small">Envío gratis a todo Ecuador. Retiro en tienda en 48h.</p>
                        </div>
                        <div class="info-section">
                            <div class="info-title"><i class="fas fa-shield-alt"></i> Garantías</div>
                            <p class="mb-0 small">Garantía óptica y estética. Limpieza y ajuste gratis.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- GRÁFICO DE TENDENCIAS -->
        <div class="row mt-4">
            <div class="col-12">
                <div class="stats-chart">
                    <h5><i class="fas fa-chart-line"></i> Tendencia de stock (últimos 5 días)</h5>
                    <canvas id="stockChart" width="400" height="200"></canvas>
                    <asp:Literal ID="ltlChartScript" runat="server" />
                </div>
            </div>
        </div>
    </div>

    <!-- MODAL PARA IMAGEN GRANDE -->
    <div class="modal fade" id="imageModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content bg-dark">
                <div class="modal-body text-center p-0">
                    <img id="modalImage" src="" style="max-width: 100%; max-height: 80vh;" />
                </div>
                <div class="modal-footer border-0 justify-content-center">
                    <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
    <script>
        function setModalImage(src) {
            document.getElementById('modalImage').src = src;
        }

        function initCarouselThumbnails() {
            const carouselElement = document.getElementById('productCarousel');
            if (!carouselElement) return;

            let carousel = bootstrap.Carousel.getInstance(carouselElement);
            if (!carousel) {
                carousel = new bootstrap.Carousel(carouselElement, { interval: false, ride: false });
            }

            // Click en miniaturas -> navegar al slide correspondiente
            document.querySelectorAll('.thumbnail').forEach(thumb => {
                thumb.addEventListener('click', function () {
                    const slideTo = parseInt(this.getAttribute('data-slide-to'));
                    if (!isNaN(slideTo)) {
                        carousel.to(slideTo);
                    }
                });
            });

            // Al cambiar de slide, actualizar la miniatura activa
            carouselElement.addEventListener('slid.bs.carousel', function (event) {
                const newIndex = event.to;
                document.querySelectorAll('.thumbnail').forEach((thumb, idx) => {
                    if (idx === newIndex) thumb.classList.add('active');
                    else thumb.classList.remove('active');
                });
            });

            // Marcar la primera miniatura como activa
            const firstThumb = document.querySelector('.thumbnail');
            if (firstThumb) firstThumb.classList.add('active');
        }

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initCarouselThumbnails);
        } else {
            initCarouselThumbnails();
        }
    </script>
</asp:Content>