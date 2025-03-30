document.getElementById("btnFindNearby").addEventListener("click", function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var latitude = position.coords.latitude;
            var longitude = position.coords.longitude;

            fetch(`/Index?handler=Nearby&lat=${latitude}&lon=${longitude}`)
                .then(response => response.json())
                .then(data => {
                    console.log("🔵 Dữ liệu từ API:", data); // Log dữ liệu trả về

                    let resultDiv = document.getElementById("result");
                    resultDiv.innerHTML = "";

                    if (data.medicals && data.medicals.length > 0) {
                        data.medicals.forEach(hospital => {
                            resultDiv.innerHTML += `
                                <div class="col-md-6 col-lg-4 col-xl-3 wow fadeInUp" data-wow-delay="0.1s">
                                    <div class="row-cols-1 feature-item p-4">
                                        <div class="col-12">
                                            <div class="feature-icon mb-4">
                                                <div class="p-3 d-inline-flex bg-white rounded">
                                                    <i class="fas fa-hospital-alt fa-4x text-primary"></i>
                                                </div>
                                            </div>
                                            <div class="feature-content d-flex flex-column">
                                                <h5 class="mb-4">${hospital.name}</h5>
                                                <p class="mb-1"><strong>Adredd:</strong> ${hospital.address}</p>
                                                <p class="mb-1"><strong>Phone number:</strong> ${hospital.phone}</p>
                                                <p class="mb-1"><strong>Email:</strong> ${hospital.email || "Không có"}</p>
                                                <p class="mb-1"><strong>Services:</strong> ${hospital.services || "Chưa cập nhật"}</p>
                                                <p class="mb-1"><strong>Opening Hours:</strong> ${hospital.openingHours || "Chưa cập nhật"}</p>
                                                <p class="mb-0"><strong>Ratings:</strong> ⭐${hospital.rating}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                        });
                    } else {
                        console.warn("🔴 Không có dữ liệu trong medicals");
                        resultDiv.innerHTML = "<p class='text-danger'>Không tìm thấy bác sĩ gần bạn.</p>";
                    }
                })
                .catch(error => console.error("🔴 Lỗi fetch API:", error));
        }, function (error) {
            alert("Không thể lấy vị trí của bạn.");
        });
    } else {
        alert("Trình duyệt không hỗ trợ lấy vị trí.");
    }
});
