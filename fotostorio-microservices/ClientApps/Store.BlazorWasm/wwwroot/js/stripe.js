function setupStripe(publishable_key) {

    let stripe = Stripe(publishable_key);

    let elements = stripe.elements();

    let style = {
        base: {
            iconColor: "#111750",
            color: "#111750",
            fontFamily: 'Roboto, Open Sans, Segoe UI, sans-serif',
            fontSmoothing: "antialiased",
            fontSize: "16px",
            "::placeholder": {
                color: "#111750"
            }
        },
        invalid: {
            iconColor: "#FF2A57",
            color: "#FF2A57"
        }
    };

    let card = elements.create('card', { style: style, hidePostalCode: true });

    mountCard(card);

    card.on('change', function (event) {
        document.querySelector("#btnPlaceOrder").disabled = event.empty;
        document.querySelector("#card-error").textContent = event.error ? event.error.message : "";
    });
};

const mountCard = (card) => {
    let checkExists = setInterval(function () {
        if (document.querySelector("#card-element") !== null) {
            card.mount("#card-element")
            clearInterval(checkExists);
        }
    }, 100);
};
