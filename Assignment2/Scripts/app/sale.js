function saleItem(idProduct, qtd, price, qtdAvailable) {
    this.IdProduct = idProduct;
    this.Qtd = qtd;
    this.price = price;
    this.productName = '';
    this.total = 0;
    this.QtdAvailable = qtdAvailable;

    this.updateTotal = function () {
        this.total = Math.round(this.Qtd * this.price * 100) / 100;
    };
}

angular.module('saleModule', [])
    .controller('saleController', function ($scope, $http) {
        $scope.isProcessing = false;

        $scope.items = [];
        $scope.actionText = 'Add To List';
        $scope.currentItem = new saleItem(0, 0, 0);
        $scope.errorMessage = '';
        $scope.classError = 'text-danger';
        $scope.showLoadingMessage = false;
        $scope.date;

        $scope.getQttForCurrentItems = function () {
            var qtt = 0;

            $scope.items.forEach(r => {
                if (r === $scope.currentItem) {
                    qtt -= r.Qtd;
                }
                if (r.IdProduct === $scope.currentItem.IdProduct) {
                    qtt += r.Qtd;
                }
            });
            console.log(qtt);
            return qtt;
        }

        $scope.verifyQttAvailable = function () {
            if ($scope.currentItem.Qtd + $scope.getQttForCurrentItems() > $scope.currentItem.QtdAvailable) {
                $scope.errorMessage = 'For this product, the quantity available is ' + $scope.currentItem.QtdAvailable;
                $scope.currentItem.Qtd = $scope.currentItem.QtdAvailable - $scope.getQttForCurrentItems();
                $scope.currentItem.updateTotal();
                return false;
            }
            return true;
        };

        $scope.updateTotal = function () {
             if($scope.verifyQttAvailable()) {
                $scope.currentItem.updateTotal();
                $scope.errorMessage = '';
            }
        };

        $scope.addItem = function () {
            if ($scope.verifyQttAvailable()) {
                if ($scope.currentItem.Qtd > 0) {
                    if ($scope.items.indexOf($scope.currentItem) < 0) {
                        $scope.items.push($scope.currentItem);
                    }

                    $scope.currentItem = new saleItem(0, 1, 0, 999);
                    $scope.actionText = 'Add New Item';
                    $scope.errorMessage = '';
                } else {
                    $scope.errorMessage = "Please inform a valid quantity!";
                }
            }
        };

        $scope.removeItem = function (item) {
            $scope.items = $scope.items.filter(r => r != item);
        };

        $scope.editItem = function (item) {
            $scope.currentItem = item;
            $scope.actionText = 'Save Item';
        };

        $scope.getTotal = function () {
            let total = 0;

            $scope.items.forEach(r => total += r.total);

            return total;
        }

        $scope.clearPurchase = function () {
            $scope.items = [];
            $scope.showLoadingMessage = false;
            $scope.isProcessing = false;
        }

        $scope.validateFields = function () {
            if ($scope.date == null || $scope.date == '') {
                $scope.errorMessage = "You must specify the date of purchase.";
            }
            else if ($scope.items.length == 0) {
                $scope.errorMessage = "You must add at least one item to your list!";
            } else {
                $scope.errorMessage = '';
            } 

            return $scope.errorMessage === '';
        };

        $scope.finishPurchase = function () {
            if (!$scope.showLoadingMessage) {
                if ($scope.validateFields()) {
                    $scope.showLoadingMessage = true;
                    $scope.isProcessing = true;

                    $http({
                        method: "POST",
                        url: "/Sale/FinishPurchase",
                        data: { date: $scope.date, items: $scope.items }
                    }).then(function success(response) {
                        $scope.isProcessing = false;
                        if (!response.data.Success) {
                            $scope.showLoadingMessage = false;
                            $scope.errorMessage = response.data.Message;
                        }
                    }, function myError(response) {
                        $scope.showLoadingMessage = false;
                        $scope.isProcessing = false;
                    });
                }
            }
        }

    });