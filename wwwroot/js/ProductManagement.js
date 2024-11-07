$(document).ready(function () {
    // Initialize DataTable with AJAX source
    $('#productTable').DataTable({
        ajax: {
            url: "/api/ProductsAPI", // API endpoint to fetch products
            dataSrc: "" // DataTables expects an array by default, so set dataSrc to an empty string
        },
        columns: [
            { data: "productId" },
            { data: "name" },
            { data: "description" },
            { data: "price" },
            { data: "quantityInStock" },
            { data: "discountPercentage" },
            {
                data: null,
                render: function (data, type, row) {
                    const finalPrice = row.price - (row.price * (row.discountPercentage / 100));
                    return finalPrice.toFixed(2);
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                                <button class="btn btn-info btn-sm" onclick="viewProduct(${row.productId})">View</button>
                                <button class="btn btn-warning btn-sm" onclick="editProduct(${row.productId})">Edit</button>
                                        <button class="btn btn-danger btn-sm" onclick="openDeleteConfirmationModal(${row.productId})">Delete</button>
                            `;
                }
            }
        ]
    });
});


function editProduct(id) {
    $.ajax({
        url: `/api/ProductsAPI/${id}`,
        type: "GET",
        success: function (product) {
            // Set the modal title to "Edit Product"
            document.getElementById("productModalLabel").innerText = "Edit Product";

            // Populate form fields with product data
            document.getElementById("productId").value = product.productId;
            document.getElementById("name").value = product.name;
            // document.getElementById("description").value = product.description;
            document.getElementById("price").value = product.price;
            document.getElementById("quantityInStock").value = product.quantityInStock;
            document.getElementById("discountPercentage").value = product.discountPercentage;
            updateFinalPrice();

            // Open the modal
            $('#productModal').modal('show');
        },
        error: function () {
            alert("An error occurred while fetching the product for editing.");
        }
    });
}

function saveOrUpdateProduct() {
    const productId = document.getElementById("productId").value; // Get the hidden product ID
    const url = productId ? `/api/ProductsAPI/${productId}` : "/api/ProductsAPI"; // Determine URL
    const method = productId ? "PUT" : "POST"; // Determine HTTP method

    console.log(method);

    const productData = {
        productId: productId || undefined, // Only include if updating
        name: document.getElementById("name").value,
        // description: document.getElementById("description").value,
        price: parseFloat(document.getElementById("price").value) || null,
        quantityInStock: parseInt(document.getElementById("quantityInStock").value) || 0,
        discountPercentage: parseFloat(document.getElementById("discountPercentage").value) || 0,
    };

    // Clear previous error messages
    document.getElementById("nameError").innerText = "";
    // document.getElementById("descriptionError").innerText = "";
    document.getElementById("priceError").innerText = "";
    document.getElementById("quantityError").innerText = "";
    document.getElementById("discountError").innerText = "";

    $.ajax({
        url: url,
        type: method,
        contentType: "application/json",
        data: JSON.stringify(productData),
        success: function () {
            $('#productModal').modal('hide'); // Close modal on success
            $('#productTable').DataTable().ajax.reload(); // Reload DataTable to show updated data
            alert(`Product ${productId ? "updated" : "added"} successfully.`);
        },
        error: function (xhr) {
            if (xhr.status === 400) {
                const errors = xhr.responseJSON.errors;

                if (errors.Name) {
                    document.getElementById("nameError").innerText = errors.Name[0];
                }
                if (errors.Description) {
                    document.getElementById("descriptionError").innerText = errors.Description[0];
                }
                if (errors.Price) {
                    document.getElementById("priceError").innerText = errors.Price[0];
                }
                if (errors.QuantityInStock) {
                    document.getElementById("quantityError").innerText = errors.QuantityInStock[0];
                }
                if (errors.DiscountPercentage) {
                    document.getElementById("discountError").innerText = errors.DiscountPercentage[0];
                }
            } else {
                alert("An error occurred while saving the product.");
            }
        }
    });
}

document.getElementById("price").addEventListener("input", updateFinalPrice);
document.getElementById("discountPercentage").addEventListener("input", updateFinalPrice);

function updateFinalPrice() {
    const price = parseFloat(document.getElementById("price").value) || 0;
    const discount = parseFloat(document.getElementById("discountPercentage").value) || 0;

    // Calculate final price based on price and discount
    const finalPrice = price - (price * discount / 100);
    document.getElementById("finalPrice").value = finalPrice.toFixed(2);
}

function resetModal() {
    document.getElementById("productModalLabel").innerText = "Add Product";
    document.getElementById("productId").value = "";
    document.getElementById("name").value = "";
    // document.getElementById("description").value = "";
    document.getElementById("price").value = "";
    document.getElementById("quantityInStock").value = "";
    document.getElementById("discountPercentage").value = "";
    document.getElementById("nameError").innerText = "";
    document.getElementById("descriptionError").innerText = "";
    document.getElementById("priceError").innerText = "";
    document.getElementById("quantityError").innerText = "";
    document.getElementById("discountError").innerText = "";
}

$('#productModal').on('hidden.bs.modal', function () {
    resetModal();
});

// delete

let productIdToDelete = null; // Holds the ID of the product to delete

// Function to open the delete confirmation modal
function openDeleteConfirmationModal(id) {
    productIdToDelete = id; // Store the ID of the product to delete
    $('#deleteConfirmationModal').modal('show'); // Open the confirmation modal
}

// Function to delete the product after confirmation
function confirmDelete() {
    if (productIdToDelete) {
        $.ajax({
            url: `/api/ProductsAPI/${productIdToDelete}`, // API endpoint with the product ID
            type: "DELETE",
            success: function (response) {
                $('#deleteConfirmationModal').modal('hide'); // Close the modal
                alert(response); // Show success message
                $('#productTable').DataTable().ajax.reload(); // Reload DataTable to refresh the product list
            },
            error: function (xhr) {
                $('#deleteConfirmationModal').modal('hide'); // Close the modal
                if (xhr.status === 404) {
                    alert("Product not found. It may have already been deleted.");
                } else {
                    alert("An error occurred while deleting the product. Please try again later.");
                }
            }
        });
    }
}

// Attach the confirm delete function to the confirm button in the modal
document.getElementById("confirmDeleteButton").addEventListener("click", confirmDelete);