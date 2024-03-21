use stylist::Style;
use web_sys::Event;
use yew::{function_component, html, Callback, Html, Properties};

const STYLE: &str = include_str!("styles/textInput.css");

#[derive(Properties, PartialEq, Clone)]
pub struct Props {
    pub label: String,
    pub input_type: String,
    pub value: String,
    pub onchange: Callback<Event>,
}

#[function_component(TextInput)]
pub fn text_input(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();

    html! {
        <div class={style}>
            <label for={props.label.clone()}>{props.label.clone()}</label>
            <input
                type={props.input_type.clone()}
                value={props.value.clone()}
                onchange={props.onchange.clone()}
                required={true}
            />
        </div>
    }
}
