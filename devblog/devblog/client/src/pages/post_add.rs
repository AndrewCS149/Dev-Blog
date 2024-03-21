use crate::{components::markdown::Markdown, helpers, store::Store, Api};
use gloo_net::http::Method;
use std::ops::Deref;
use stylist::{css, Style};
use web_sys::{FileList, FormData, HtmlInputElement, HtmlTextAreaElement};
use yew::prelude::*;
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/postAdd.css");

#[function_component(AddPost)]
pub fn add_post() -> Html {
    let style = Style::new(STYLE).unwrap();
    let files: UseStateHandle<Option<FileList>> = use_state(|| None);
    let description = use_state(|| String::default());
    let char_count = use_state(|| 0);
    let loading = use_state(|| false);
    let devblog = use_state(|| false);
    let devblog_status = use_state(|| String::default());
    let mastodon = use_state(|| false);
    let mastodon_status = use_state(|| String::default());
    let discord = use_state(|| false);
    let discord_status = use_state(|| String::default());
    let err = use_state(|| String::default());
    let store = use_store_value::<Store>();

    let char_count_clone = char_count.clone();
    let description_clone = description.clone();
    use_effect_with(description_clone.clone(), move |_| {
        char_count_clone.set(description_clone.len());
    });

    // create post
    let onsubmit = {
        let token = store.token.clone();
        let description = description.clone();
        let err = err.clone();
        let files = files.clone();
        let loading = loading.clone();

        let discord = *discord.clone();
        let discord_status = discord_status.clone();

        let mastodon = *mastodon.clone();
        let mastodon_status = mastodon_status.clone();

        let devblog = *devblog.clone();
        let devblog_status = devblog_status.clone();

        Callback::from(move |e: SubmitEvent| {
            e.prevent_default();
            loading.set(true);
            let loading = loading.clone();
            let err = err.clone();

            let discord_status = discord_status.clone();
            let mastodon_status = mastodon_status.clone();
            let devblog_status = devblog_status.clone();

            let hdrs = helpers::create_auth_header(&token);
            let form_data = FormData::new().unwrap();
            let _ = form_data.append_with_str("description", &description);
            let _ = form_data.append_with_str("postToDiscord", &discord.to_string());
            let _ = form_data.append_with_str("postToMastodon", &mastodon.to_string());
            let _ = form_data.append_with_str("postToDevBlog", &devblog.to_string());

            // add imgs to FileList
            if let Some(f) = files.deref() {
                for i in 0..f.length() {
                    if let Some(file) = f.item(i) {
                        let _ = form_data.append_with_blob("files", &file).unwrap();
                    }
                }
            }

            wasm_bindgen_futures::spawn_local(async move {
                let response = Api::AddPost
                    .fetch(Some(hdrs), Some(form_data.into()), Method::POST)
                    .await;

                if let Some(res) = response {
                    let status = format!("{}: {}", res.status(), res.status_text());

                    if discord {
                        discord_status.set(status.clone());
                    }
                    if mastodon {
                        mastodon_status.set(status.clone());
                    }
                    if devblog {
                        devblog_status.set(status);
                    }
                } else {
                    err.set("Failed to send request.".to_string());
                }

                loading.set(false);
            });
        })
    };

    // status of platform upload
    let upload_status = |platform: String, upload: bool, status: String| -> Html {
        html! {
            <h4 class={match upload {true => css!("display: block;"), false => css!("display: none;")}}>{platform}{" Upload Status: "}
                <span class={if (status).to_string().starts_with('2') {css!("color: green;")} else {css!("color: red;")}}>
                    {status}{err.deref()}
                </span>
            </h4>
        }
    };

    // checkboxes for platform upload selection
    let upload_to = |platform: String, platform_state: UseStateHandle<bool>| -> Html {
        let on_checkbox_change = {
            Callback::from(move |e: Event| {
                let checkbox = e.target_dyn_into::<HtmlInputElement>().unwrap();
                platform_state.set(checkbox.checked());
            })
        };

        html! {
          <li>
              <label>
                  <input
                  type="checkbox"
                  onchange={on_checkbox_change}/>
                  {platform}
              </label>
          </li>
        }
    };

    // update the character limits
    let update_char_count = {
        let description = description.clone();
        Callback::from(move |e: InputEvent| {
            let input = e.target_dyn_into::<HtmlTextAreaElement>();
            description.set(input.unwrap().value());
        })
    };

    // set files for upload
    let update_imgs = {
        let files = files.clone();
        Callback::from(move |e: Event| {
            let input = e.target_dyn_into::<HtmlInputElement>();
            if let Some(f) = input {
                let selected_files = f.files().unwrap();
                files.set(Some(selected_files));
            }
        })
    };

    html! {
        if store.admin {
            <section class={style}>
                <div class="create-post">
                    <form {onsubmit}>
                        <div class="upload-platforms">

                            // PLATFORM UPLOAD OPTIONS
                            <p>{"Upload to:"}</p>
                            <ul>
                                {upload_to("Discord".to_string(), discord.clone())}
                                {upload_to("Mastodon".to_string(), mastodon.clone())}
                                {upload_to("DevBlog".to_string(), devblog.clone())}
                            </ul>

                            if *loading {
                                <span class="loader">{"Loading..."}</span>
                            }

                            // UPLOAD STATUSES
                            <div class="upload-status">
                                {upload_status("Discord".to_string(), *discord.clone(), discord_status.deref().clone())}
                                {upload_status("Mastodon".to_string(), *mastodon.clone(), mastodon_status.deref().clone())}
                                {upload_status("DevBlog".to_string(), *devblog.clone(), devblog_status.deref().clone())}
                            </div>
                        </div>

                        // NEW POST INFO
                        <div class="new-post-info">
                            <label>{"Image"}
                                <input type="file"
                                    required=true
                                    multiple=true
                                    onchange={update_imgs}/>
                            </label>

                            <label>{"Description"}
                                <p>{"Mastodon character limit: "}{*char_count}{"/500"}</p>
                                <p>{"Discord character limit: "}{*char_count}{"/2000"}</p>
                                <div class="addpost-description">
                                    <textarea oninput={update_char_count} placeholder="Write description here..."/>
                                </div>
                            </label>

                            <p>{"Preview:"}</p>
                            <div class="post-preview">
                                <span>{"preview content:"}</span>
                                <Markdown content={description.deref().clone()}/>
                            </div>
                            <button>{"Create Post"}</button>
                        </div>
                    </form>
                </div>
            </section>
        }
    }
}
