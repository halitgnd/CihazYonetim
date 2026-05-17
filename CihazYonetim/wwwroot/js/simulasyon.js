$(document).ready(function () {

    let arenaWidth = $(".simulasyon-alani").width();
    let arenaHeight = $(".simulasyon-alani").height();

    // 0. POZİSYON RASTGELE DAĞITICI
    $(".cihaz-daire").each(function () {
        if ($(this).css("left") === "0px" || $(this).css("left") === "auto") {
            let rastgeleX = Math.floor(Math.random() * (arenaWidth - 120));
            let rastgeleY = Math.floor(Math.random() * (arenaHeight - 120));
            $(this).css({
                "left": Math.max(10, rastgeleX) + "px",
                "top": Math.max(10, rastgeleY) + "px"
            });
        }
    });

    let surukleniyorMu = false;

    // --- 15 SANİYEDE BİR HIZI DALGALANDIR ---
    setInterval(function() {
        let rastgeleHiz = (Math.random() * (95.0 - 35.0) + 35.0).toFixed(1);
        $("#canli-hiz").text(rastgeleHiz + " Mbps");
        agBaglantilariniCiz();
    }, 15000); // 15000 milisaniye = 15 saniye (Gözü yormaz, takılma yapmaz)

    // --- AĞ ÇİZİMİ VE DİNAMİK HIZ MOTORU ---
    function agBaglantilariniCiz() {
        const svg = $("#network-cizgileri");
        svg.empty();

        let hizYazisi = $("#canli-hiz").text().replace(/[^0-9.,]/g, '').replace(',', '.');
        let veriHizi = parseFloat(hizYazisi) || 50;

        let animasyonSuresi = Math.max(0.15, 50 / veriHizi);

        let routerlar = $(".cihaz-daire[data-turu='0']");
        let digerCihazlar = $(".cihaz-daire").not("[data-turu='0']");

        if (routerlar.length === 0) return;

        digerCihazlar.each(function () {
            let cihaz = $(this);
            let ucX = cihaz.position().left + (cihaz.width() / 2);
            let ucY = cihaz.position().top + (cihaz.height() / 2);
            let cihazDurum = cihaz.attr("data-durum");

            // EN YAKIN ROUTER RADARI
            let enYakinRouter = null;
            let enKisaMesafe = Infinity;

            routerlar.each(function () {
                let rX = $(this).position().left + ($(this).width() / 2);
                let rY = $(this).position().top + ($(this).height() / 2);
                let dx = rX - ucX;
                let dy = rY - ucY;
                let mesafe = Math.sqrt((dx * dx) + (dy * dy));

                if (mesafe < enKisaMesafe) {
                    enKisaMesafe = mesafe;
                    enYakinRouter = $(this);
                }
            });

            if (!enYakinRouter) return;

            let merkezX = enYakinRouter.position().left + (enYakinRouter.width() / 2);
            let merkezY = enYakinRouter.position().top + (enYakinRouter.height() / 2);
            let routerDurum = enYakinRouter.attr("data-durum");

            // ANA ŞALTER KONTROLÜ
            let baglantiAktifMi = (cihazDurum == 1 && routerDurum == 1);

            let cizgiRengi = baglantiAktifMi ? "rgba(46, 204, 113, 0.8)" : "rgba(255, 255, 255, 0.1)";
            let kalinlik = baglantiAktifMi ? "3" : "1";

            let line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
            line.setAttribute('x1', merkezX);
            line.setAttribute('y1', merkezY);
            line.setAttribute('x2', ucX);
            line.setAttribute('y2', ucY);
            line.setAttribute('stroke', cizgiRengi);
            line.setAttribute('stroke-width', kalinlik);

            if (baglantiAktifMi) {
                line.setAttribute('stroke-dasharray', '8, 8');
                line.setAttribute('class', 'cizgi-aktif');
                line.style.animationDuration = animasyonSuresi + "s";
            }

            svg.append(line);
        });
    }

    agBaglantilariniCiz();

    // SÜRÜKLEME
    $(".cihaz-daire").draggable({
        containment: ".simulasyon-alani",
        scroll: false,
        start: function() {
            surukleniyorMu = true;
            $(this).css("z-index", 1000);
        },
        drag: function() { agBaglantilariniCiz(); },
        stop: function(event, ui) {
            $(this).css("z-index", 100);
            let cihazId = $(this).attr("data-id");
            let yeniX = ui.position.left;
            let yeniY = ui.position.top;

            $.post('/Cihaz/PozisyonGuncelle', { cihazId: cihazId, x: yeniX, y: yeniY });
            setTimeout(function() { surukleniyorMu = false; }, 100);
        }
    });

    // NAVBAR
    function navbarGuncelle(eskiDurum, yeniDurum) {
        if(eskiDurum === yeniDurum) return;

        const idMap = { 0: "#nav-offline", 1: "#nav-online", 2: "#nav-ariza", 3: "#nav-bakim" };
        let eskiSayacEl = $(idMap[eskiDurum]);
        let yeniSayacEl = $(idMap[yeniDurum]);

        if(eskiSayacEl.length) eskiSayacEl.text(Math.max(0, (parseInt(eskiSayacEl.text()) || 0) - 1));
        if(yeniSayacEl.length) yeniSayacEl.text((parseInt(yeniSayacEl.text()) || 0) + 1);
    }

    // TIKLAMA VE DURUM DEĞİŞTİRME
    $(document).on("click", ".cihaz-icerik", function (e) {
        if (surukleniyorMu) return;

        let top = $(this).closest(".cihaz-daire");
        let cihazId = top.attr("data-id");
        let mevcutDurum = parseInt(top.attr("data-durum"));
        let yeniDurum = (mevcutDurum === 1) ? 0 : 1;

        navbarGuncelle(mevcutDurum, yeniDurum);

        if (yeniDurum === 1) {
            top.css({"background-color": "#198754", "box-shadow": "0 0 20px #198754"});
            top.find(".daire-merkez").text("Online");
        } else {
            top.css({"background-color": "#6c757d", "box-shadow": "none"});
            top.find(".daire-merkez").text("Offline");
        }

        top.attr("data-durum", yeniDurum);
        durumDegistirAJAX(cihazId, yeniDurum);
        agBaglantilariniCiz();
    });

    // ARIZA BUTONU [A]
    $(document).on("click", ".ariza-btn", function (e) {
        if (surukleniyorMu) return;

        let top = $(this).closest(".cihaz-daire");
        let cihazId = top.attr("data-id");
        let mevcutDurum = parseInt(top.attr("data-durum"));

        if (mevcutDurum !== 2) {
            navbarGuncelle(mevcutDurum, 2);
            top.css({"background-color": "#dc3545", "box-shadow": "0 0 25px #dc3545"});
            top.find(".daire-merkez").text("Arıza");
            top.attr("data-durum", 2);
            durumDegistirAJAX(cihazId, 2);
            agBaglantilariniCiz();
        }
    });

    // BAKIM BUTONU [B]
    $(document).on("click", ".bakim-btn", function (e) {
        if (surukleniyorMu) return;

        let top = $(this).closest(".cihaz-daire");
        let cihazId = top.attr("data-id");
        let mevcutDurum = parseInt(top.attr("data-durum"));

        if (mevcutDurum !== 3) {
            navbarGuncelle(mevcutDurum, 3);
            top.css({"background-color": "#fd7e14", "box-shadow": "0 0 20px #fd7e14"});
            top.find(".daire-merkez").text("Bakım");
            top.attr("data-durum", 3);
            durumDegistirAJAX(cihazId, 3);
            agBaglantilariniCiz();
        }
    });

    function durumDegistirAJAX(id, yeniDurumId) {
        $.ajax({
            url: '/Cihaz/DurumGuncelle',
            type: 'POST',
            data: { cihazId: id, yeniDurum: yeniDurumId },
            error: function () { console.log("AJAX Hatası: Durum güncellenemedi."); }
        });
    }
});
$(document).ready(function () {
    // 1. PANELİ AÇIP KAPATAN TETİKÇİ
    $(".btn-yeni").on("click", function(e) {
        e.preventDefault();
        let panel = $("#yeniCihazPaneli");
        if (panel.is(":visible")) {
            panel.fadeOut(200);
        } else {
            panel.fadeIn(200).css("display", "flex");
        }
    });

    // 2. AĞ ÇİZİM MOTORU
    function agBaglantilariniCiz() {
        const svg = $("#network-cizgileri");
        svg.empty();
        let veriHizi = parseFloat($("#canli-hiz").text()) || 50;
        let animSuresi = Math.max(0.2, 50 / veriHizi);

        let routerlar = $(".cihaz-daire[data-turu='0']");
        let cihazlar = $(".cihaz-daire").not("[data-turu='0']");

        cihazlar.each(function () {
            let cihaz = $(this);
            let cX = cihaz.position().left + 50;
            let cY = cihaz.position().top + 50;
            let enYakin = null; let minMesafe = Infinity;

            routerlar.each(function () {
                let rX = $(this).position().left + 50;
                let rY = $(this).position().top + 50;
                let d = Math.sqrt(Math.pow(rX-cX, 2) + Math.pow(rY-cY, 2));
                if (d < minMesafe) { minMesafe = d; enYakin = $(this); }
            });

            if (enYakin) {
                let rX = enYakin.position().left + 50;
                let rY = enYakin.position().top + 50;
                let aktif = (cihaz.attr("data-durum") == 1 && enYakin.attr("data-durum") == 1);

                let line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                line.setAttribute('x1', rX); line.setAttribute('y1', rY);
                line.setAttribute('x2', cX); line.setAttribute('y2', cY);
                line.setAttribute('stroke', aktif ? "rgba(46, 204, 113, 0.8)" : "rgba(255, 255, 255, 0.1)");
                line.setAttribute('stroke-width', aktif ? "3" : "1");
                if (aktif) {
                    line.setAttribute('stroke-dasharray', '8, 8');
                    line.setAttribute('class', 'cizgi-aktif');
                    line.style.animationDuration = animSuresi + "s";
                }
                svg.append(line);
            }
        });
    }

    // Sürükleme ve Diğerleri
    $(".cihaz-daire").draggable({
        containment: ".simulasyon-alani",
        drag: function() { agBaglantilariniCiz(); },
        stop: function(e, ui) {
            $.post('/Cihaz/PozisyonGuncelle', { cihazId: $(this).attr("data-id"), x: ui.position.left, y: ui.position.top });
        }
    });

    agBaglantilariniCiz();
});