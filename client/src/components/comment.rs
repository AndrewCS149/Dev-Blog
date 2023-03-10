use serde::Deserialize;
use yew::prelude::*;

#[derive(Deserialize, Clone, PartialEq, Properties)]
pub struct CommentProps {
    pub id: i32,
    pub content: String,
    pub date: String,
    pub userName: String,
}

#[function_component]
pub fn Comment(props: &CommentProps) -> Html {
    html! {
        <div>
            <div>
                <span>{&props.userName}</span>
                <span>{&props.date}</span>
            </div>

            <p>{&props.content}</p>
        </div>
    }
}
